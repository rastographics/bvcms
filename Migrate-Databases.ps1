param (
    [string]$scripts = ".\CmsData\Migrations",
    [switch]$prod = $false,
    [string]$db = $null,
    [string]$dbname = $null,
    [int]$start = 0,
    [int]$end = 10000
)

$logfile = "Migrate-Databases-$start.log"
$page = $end - $start
$Global:SERVERNAME = if ($prod) {$env:ProductionDatabaseServer} else {"(local)"}
$Global:SQLUSERNAME = $env:ProductionDatabaseUser
$Global:SQLPASSWORD = $env:ProductionDatabasePassword

function Get-Connection {
    param(
        [string] $dbname
    )
    try {
        $connection = New-Object System.Data.SQLClient.SQLConnection
        if ($prod) {
            $connection.ConnectionString = "server=$SERVERNAME;database=$dbname;User Id=$SQLUSERNAME;Password=$SQLPASSWORD"
        }
        else {
            $connection.ConnectionString = "server=$SERVERNAME;database=$dbname;Integrated Security=true"
        }
        $connection.Open()
    }
    catch {
        Write-Log "Failed to connect to SQL Server: $PSItem"
        exit
    }
    return $connection
}

function Get-QueryResult {
    param(
        [System.Data.SqlClient.SqlConnection] $connection, 
        [string] $query
    )
    $command = New-Object System.Data.SqlClient.SqlCommand
    $command.CommandText = $query
    $command.Connection = $connection

    $adapter = New-Object System.Data.SqlClient.SqlDataAdapter
    $adapter.SelectCommand = $command
    $dataset = New-Object System.Data.DataSet
    $adapter.fill($dataset) | out-null
    return $dataset
}

function Get-AllSQLScripts {
    param (
        [string] $folder
    )
    Write-Log "Loading scripts from $folder"
    $list = New-Object System.Collections.Generic.List[object]
    Get-ChildItem $folder -Filter *.sql | Foreach-Object {
        $cmds = New-Object System.Collections.Generic.List[string]
        Write-Log $_.Name
        $content = [IO.File]::ReadAllText($_.FullName)
        $content -split '^GO.*$', 0, "multiline" | Foreach-Object {
            $cmds.Add($_)
        }
        $o = @{}
        $o.name = $_.Name
        $o.commands = $cmds
        $list.Add($o)
    }
    
    return $list
}

function Get-IsMigrationApplied {
    param (
        [string] $id,
        [System.Data.DataSet] $migrations
    )
    foreach ($data in $migrations.tables[0]) {
        if ($id -eq $data[0]) {
            return $true
        }
    }
    return $false
}

function Invoke-SqlCommand {
    param (
        [System.Data.SqlClient.SqlConnection] $connection,
        [string] $commandText
    )
    
    $command = New-Object System.Data.SqlClient.SqlCommand
    $command.Connection = $connection
    $command.CommandTimeout = 120
    $command.CommandText = $commandText
    return $command.ExecuteNonQuery() | Out-Null
}

function Write-Log {
    param (
        [string] $message
    )
    Write-Host $message
    "$message" | Out-File -FilePath $logfile -Append
}

if ((Get-Item $logfile).length -gt 2mb) {
    Remove-Item $logfile
}
Write-Log (Get-Date).toString("yyyy-MM-dd HH:mm:ss")
$msg = if ($prod) {"Connecting to Production"} else {"Local connection"}
Write-Log $msg

$commandsToRun = Get-AllSQLScripts $scripts

$db = if ($db -eq $null) { $dbname } else { $db }
$SQLConnection = Get-Connection "master"
$SQLDataset = Get-QueryResult $SQLConnection "SELECT name FROM sys.databases WHERE name LIKE 'CMS[_]%' AND (LEN('$db') = 0 OR name='$db') ORDER BY name OFFSET $start ROWS FETCH NEXT $page ROWS ONLY"
$SQLConnection.close()

foreach ($data in $SQLDataset.tables[0]) {
    $cmsdbname = $data[0]
    Write-Log $cmsdbname
    $connection = Get-Connection $cmsdbname
    $migrations = Get-QueryResult $connection "IF EXISTS (SELECT 1 FROM sys.tables WHERE name = '__SqlMigrations') SELECT Id FROM dbo.__SqlMigrations"
    foreach($cmd in $commandsToRun) {
        $id = $cmd.name
        $commands = $cmd.commands
        $applied = Get-IsMigrationApplied $id $migrations
        if (-Not $applied) {
            $commands | Foreach-Object {
                if ($_.Trim().Length -gt 0) {
                    try {
                        Invoke-SqlCommand $connection $_ -ErrorAction Stop
                    } catch { 
                        Write-Error "Error in $id"
                        Write-Error $_
                        Write-Log "$cmsdbname" "$id" "$_" 
                        break
                    }
                }
            }
            Write-Log "Applied $id"
            Invoke-SqlCommand $connection "INSERT INTO dbo.__SqlMigrations (Id) VALUES('$id')"
        }
    }
}