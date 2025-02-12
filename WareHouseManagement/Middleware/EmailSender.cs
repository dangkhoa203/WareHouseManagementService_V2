using System.Net.Mail;
using System.Net;

namespace WareHouseManagement.Middleware
{
    public class EmailSender
    {
        public static async Task<bool> SendEmail(string toEmail,string subject,string body)
        {
            try
            {
                using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
                {
                    client.Port = 587;
                    client.Credentials = new NetworkCredential("dkwebsoftware@gmail.com", "tnmx nqah rtlq ekwp");
                    client.EnableSsl = true;
                    using (var message = new MailMessage("dkwebsoftware@gmail.com", toEmail))
                    {
                        message.Subject = subject;
                        message.Body = body;
                        message.BodyEncoding = System.Text.Encoding.UTF8;
                        message.SubjectEncoding = System.Text.Encoding.UTF8;
                        message.IsBodyHtml = true;
                        await client.SendMailAsync(message);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static async Task<bool> SendConfirmEmail(string toEmail, string subject,string content, string confirmLink,string linkAction) {
            try {
                string body = $@"
                  <!doctype html>
<html xmlns=""http://www.w3.org/1999/xhtml"" xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:o=""urn:schemas-microsoft-com:office:office"">

<head>
  <title>
  </title>
  <!--[if !mso]><!-->
  <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
  <!--<![endif]-->
  <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
  <style type=""text/css"">
    #outlook a {{
      padding: 0;
    }}

    body {{
      margin: 0;
      padding: 0;
      -webkit-text-size-adjust: 100%;
      -ms-text-size-adjust: 100%;
    }}

    table,
    td {{
      border-collapse: collapse;
      mso-table-lspace: 0pt;
      mso-table-rspace: 0pt;
    }}

    img {{
      border: 0;
      height: auto;
      line-height: 100%;
      outline: none;
      text-decoration: none;
      -ms-interpolation-mode: bicubic;
    }}

    p {{
      display: block;
      margin: 13px 0;
    }}
  </style>
  <!--[if mso]>
        <noscript>
        <xml>
        <o:OfficeDocumentSettings>
          <o:AllowPNG/>
          <o:PixelsPerInch>96</o:PixelsPerInch>
        </o:OfficeDocumentSettings>
        </xml>
        </noscript>
        <![endif]-->
  <!--[if lte mso 11]>
        <style type=""text/css"">
          .mj-outlook-group-fix {{ width:100% !important; }}
        </style>
        <![endif]-->
  <!--[if !mso]><!-->
  <link href=""https://fonts.googleapis.com/css?family=Ubuntu:300,400,500,700"" rel=""stylesheet"" type=""text/css"">
  <style type=""text/css"">
    @import url(https://fonts.googleapis.com/css?family=Ubuntu:300,400,500,700);
  </style>
  <!--<![endif]-->
  <style type=""text/css"">
    @media only screen and (min-width:480px) {{
      .mj-column-per-100 {{
        width: 100% !important;
        max-width: 100%;
      }}
    }}
  </style>
  <style media=""screen and (min-width:480px)"">
    .moz-text-html .mj-column-per-100 {{
      width: 100% !important;
      max-width: 100%;
    }}
  </style>
  <style type=""text/css"">
  </style>
</head>

<body style=""word-spacing:normal;background-color:#F4F4F4;"">
  <div style=""background-color:#F4F4F4;"">
    <!--[if mso | IE]><table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class="""" style=""width:600px;"" width=""600"" bgcolor=""#1155c2"" ><tr><td style=""line-height:0px;font-size:0px;mso-line-height-rule:exactly;""><![endif]-->
    <div style=""background:#1155c2;background-color:#1155c2;margin:0px auto;max-width:600px;"">
      <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""background:#1155c2;background-color:#1155c2;width:100%;"">
        <tbody>
          <tr>
            <td style=""direction:ltr;font-size:0px;padding:20px 0;text-align:center;"">
              <!--[if mso | IE]><table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0""><tr><td class="""" style=""vertical-align:top;width:600px;"" ><![endif]-->
              <div class=""mj-column-per-100 mj-outlook-group-fix"" style=""font-size:0px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%;"">
                <table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""vertical-align:top;"" width=""100%"">
                  <tbody>
                    <tr>
                      <td align=""center"" style=""font-size:0px;padding:10px 25px;word-break:break-word;"">
                        <div style=""font-family:Arial, sans-serif;font-size:40px;line-height:1;text-align:center;text-transform:uppercase;color:white;"">Dịch vụ quản lý kho</div>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
              <!--[if mso | IE]></td></tr></table><![endif]-->
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <!--[if mso | IE]></td></tr></table><table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class="""" style=""width:600px;"" width=""600"" bgcolor=""#ffffff"" ><tr><td style=""line-height:0px;font-size:0px;mso-line-height-rule:exactly;""><![endif]-->
    <div style=""background:#ffffff;background-color:#ffffff;margin:0px auto;max-width:600px;"">
      <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""background:#ffffff;background-color:#ffffff;width:100%;"">
        <tbody>
          <tr>
            <td style=""direction:ltr;font-size:0px;padding:20px 0px 20px 0px;text-align:center;"">
              <!--[if mso | IE]><table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0""><tr><td class="""" style=""vertical-align:top;width:600px;"" ><![endif]-->
              <div class=""mj-column-per-100 mj-outlook-group-fix"" style=""font-size:0px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%;"">
                <table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""vertical-align:top;"" width=""100%"">
                  <tbody>
                    <tr>
                      <td align=""center"" style=""font-size:0px;padding:0px 25px 0px 25px;word-break:break-word;"">
                        <div style=""font-family:Arial, sans-serif;font-size:14px;line-height:28px;text-align:center;color:#55575d;"">{content}</div>
                      </td>
                    </tr>
                    <tr>
                      <td align=""center"" vertical-align=""middle"" style=""font-size:0px;padding:10px 25px;word-break:break-word;"">
                        <table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""border-collapse:separate;line-height:100%;"">
                          <tr>
                            <td align=""center"" bgcolor=""#47a66d"" role=""presentation"" style=""border:none;border-radius:3px;cursor:auto;mso-padding-alt:10px 25px;background:#47a66d;"" valign=""middle"">
                              <a href={confirmLink} style=""display:inline-block;background:#47a66d;color:#ffffff;font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:13px;font-weight:normal;line-height:120%;margin:0;text-decoration:none;text-transform:none;padding:10px 25px;mso-padding-alt:0px;border-radius:3px;""> {linkAction} </a>
                            </td>
                          </tr>
                        </table>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
              <!--[if mso | IE]></td></tr></table><![endif]-->
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <!--[if mso | IE]></td></tr></table><table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class="""" style=""width:600px;"" width=""600"" bgcolor=""#1155c2"" ><tr><td style=""line-height:0px;font-size:0px;mso-line-height-rule:exactly;""><![endif]-->
    <div style=""background:#1155c2;background-color:#1155c2;margin:0px auto;max-width:600px;"">
      <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""background:#1155c2;background-color:#1155c2;width:100%;"">
        <tbody>
          <tr>
            <td style=""direction:ltr;font-size:0px;padding:20px 0;text-align:center;"">
              <!--[if mso | IE]><table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0""><tr><td class="""" style=""vertical-align:top;width:600px;"" ><![endif]-->
              <div class=""mj-column-per-100 mj-outlook-group-fix"" style=""font-size:0px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%;"">
                <table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""vertical-align:top;"" width=""100%"">
                  <tbody>
                    <tr>
                      <td align=""left"" style=""font-size:0px;padding:10px 25px;word-break:break-word;"">
                        <div style=""font-family:Arial, sans-serif;font-size:13px;line-height:22px;text-align:left;color:#ffffff;"">©2025 DKWEBSOFT</div>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
              <!--[if mso | IE]></td></tr></table><![endif]-->
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <!--[if mso | IE]></td></tr></table><![endif]-->
  </div>
</body>

</html>
";
                using (SmtpClient client = new SmtpClient("smtp.gmail.com")) {
                    client.Port = 587;
                    client.Credentials = new NetworkCredential("dkwebsoftware@gmail.com", "tnmx nqah rtlq ekwp");
                    client.EnableSsl = true;
                    using (var message = new MailMessage("dkwebsoftware@gmail.com", toEmail)) {
                        message.Subject = subject;
                        message.Body = body;
                        message.BodyEncoding = System.Text.Encoding.UTF8;
                        message.SubjectEncoding = System.Text.Encoding.UTF8;
                        message.IsBodyHtml = true;
                        await client.SendMailAsync(message);
                    }
                    return true;
                }
            } catch (Exception ex) {
                return false;
            }
        }
    }
}

