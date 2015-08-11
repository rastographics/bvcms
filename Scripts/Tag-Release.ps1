if ((Get-Command git.exe*) -eq $null) {
    throw "git.exe wasn't found, please ensure git.exe is in your PATH"
}

$releaseDate = [DateTime]::Now.ToString("yyyy-MM-dd-HH-mm")

Write-Host "Tagging release at $releaseDate..."

git tag $releaseDate
git push --tags
