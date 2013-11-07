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
  <title>BVCMS error</title>
  <!-- css -->
  <style type="text/css">

    * {
      margin: 0;
      padding: 0;
    }
    body {
      background: #F0F0F0;
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
    <div id="surface">
    <h1>Ooops! Our web page did something wrong</h1>
    <p>We are very sorry and an email we have logged the error messsage. Please try again...but if you continue to have problems, please use the Contact Support button to let us know.</p>
    <h4><%=HttpContext.Current.Items["error"] %></h4>
    </div>
</body>
</html>
