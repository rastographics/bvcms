<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="System.Security.Cryptography" %>
<%@ Import Namespace="System.Threading" %>
<script runat="server">
   void Page_Load() {
      byte[] delay = new byte[1];
      RandomNumberGenerator prng = new RNGCryptoServiceProvider();

      prng.GetBytes(delay);
      Thread.Sleep((int)delay[0]);
        
      IDisposable disposable = prng as IDisposable;
      if (disposable != null) { disposable.Dispose(); }
   }
</script>
<!DOCTYPE html>
<html lang="en">
<head>
  <!-- meta -->
  <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
  <meta name="language" content="en-US">
  <meta name="viewport" content="maximum-scale=1.0,width=device-width,initial-scale=1.0">
  <!-- title -->
  <title>TouchPoint error</title>
  <!-- css -->
  <style type="text/css">

    * {
      margin: 0;
      padding: 0;
    }
    body {
      font: 14px/18px Arial, "Helvetica Neue", sans-serif;
      text-align: center;
    }
    #surface {
      margin: 40px auto;
      position: relative;
      width: 480px;
    }
    h1 { 
      color: #222; 
      font-size: 22px; 
      font-weight: bold; 
      line-height: 26px; 
      margin-bottom: 10px; 
    }
    p { 
      margin-bottom: 10px;
    }

    @media all and (max-width: 480px) {
      #surface {
        margin: 40px 10px;
        width: 100%;
      }
    }

  </style>
</head>

<body>
    <img src="/Content/images/brokencode.jpg" style="margin-left: auto;margin-right:auto" />    
    <div id="surface">
    <h1>Oops! It seems you stumbled on something that doesn't work very well.</h1>
        <p>
        If this problem persists, please kindly let us know.
        Thanks!
    </p>
    <h4><%=HttpContext.Current.Items["error"] %></h4>
    </div>
</body>
</html>
