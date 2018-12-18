<% Response.StatusCode = 503; %>
<%@ Page Language="C#" AutoEventWireup="True" %>
<!DOCTYPE html>
<html lang="en">
<head>
    <link rel="shortcut icon" href="/favicon.ico?v=2">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, maximum-scale=1.0">
    <title>Database Not Initialized</title>
    <link href="//fonts.googleapis.com/css?family=Open+Sans:300italic,400italic,400,600,300,700" rel="stylesheet">
    <link href="//netdna.bootstrapcdn.com/font-awesome/4.3.0/css/font-awesome.css" rel="stylesheet">
    <link href="/Content/touchpoint/css/error.css" rel="stylesheet" />
    <!-- HTML5 shim and Respond.js for IE8 support of HTML5 elements and media queries -->
    <!--[if lt IE 9]>
        <script src="https://oss.maxcdn.com/html5shiv/3.7.2/html5shiv.min.js"></script>
        <script src="https://oss.maxcdn.com/respond/1.4.2/respond.min.js"></script>
    <![endif]-->
</head>
<body class="page-500">
    <div class="header">
        <a href="/">
            <div><img class="logo" src="/Content/touchpoint/img/logo_sm.png" alt="" style="margin-top: -4px;"></div>&nbsp;
            <strong>TouchPoint</strong> Software
        </a>
    </div>
    <div class="error-code">OOPS!</div>
    <div class="error-text">
        <span class="oops">DATABASE NOT INITIALIZED</span><br>
        <span class="hr"></span>
        <br>
            Database does not appear to be initialized.
        <br>
        <span class="solve">
            Please delete both the CMS_<%=Request.QueryString["dbname"] %> and the CMSi_<%=Request.QueryString["dbname"] %> databases and start over.
        </span>
    </div>
    <div id="footer" class="container-fluid hidden-print">
        <div class="text-center">
            &copy; <%=DateTime.Today.Year %> <%=Resource1.CompanyName %>
        </div>
    </div>
    <script src='//ajax.googleapis.com/ajax/libs/jquery/1.11.2/jquery.min.js'></script>
    <script src="//maxcdn.bootstrapcdn.com/bootstrap/3.3.2/js/bootstrap.min.js"></script>
</body>
</html>
