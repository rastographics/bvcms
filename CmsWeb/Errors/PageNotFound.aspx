<% Response.StatusCode = 404; %>
<%@ Page Language="C#" AutoEventWireup="True" %>
<!DOCTYPE html>
<html lang="en">
<head>
    <link rel="shortcut icon" href="/favicon.ico?v=4">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, maximum-scale=1.0">
    <title>404</title>
    <%=ViewExtensions2.GoogleFonts() %>
    <%=ViewExtensions2.FontAwesome() %>
    <link href="<%=Request.Url.GetLeftPart(UriPartial.Authority)%>/Content/touchpoint/css/error.css" rel="stylesheet" />
    <!-- HTML5 shim and Respond.js for IE8 support of HTML5 elements and media queries -->
    <!--[if lt IE 9]>
        <script src="https://oss.maxcdn.com/html5shiv/3.7.2/html5shiv.min.js"></script>
        <script src="https://oss.maxcdn.com/respond/1.4.2/respond.min.js"></script>
    <![endif]-->
</head>
<body class="page-404">
    <div class="header">
        <a href="/">
            <div><img class="logo" src="/Content/touchpoint/img/logo_sm.png" alt="" style="margin-top: -4px;"></div>&nbsp;
            <strong>TouchPoint</strong> Software
        </a>
    </div>
    <div class="error-code">404</div>
    <div class="error-text">
        <span class="oops">PAGE NOT FOUND</span><br/>
        <span class="hr"></span>
        <br>
        SOMETHING WENT WRONG, OR THAT PAGE DOESN'T EXIST... YET
    </div>
    <div id="footer" class="container-fluid hidden-print">
        <div class="text-center">
            &copy; <%=DateTime.Today.Year %> <%=Resource1.CompanyName %>
        </div>
    </div>
    <script src='//ajax.googleapis.com/ajax/libs/jquery/1.11.2/jquery.min.js'></script>
    <%=ViewExtensions2.Bootstrap3() %>
</body>
</html>
