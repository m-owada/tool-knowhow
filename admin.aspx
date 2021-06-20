<%@ Page Language="C#" CodeFile="admin.aspx.cs" Inherits="Admin" Debug="true" %>
<!DOCTYPE html>
<html lang="ja">
  <head>
    <meta charset="UTF-8">
    <meta http-equiv="x-ua-compatible" content="IE=Edge">
    <style type="text/css">
      body
      {
        box-sizing: border-box;
        padding: 0px 10px;
      }
      .input-group input[type="submit"]
      {
        box-sizing: border-box;
      }
      .input-group textarea
      {
        box-sizing: border-box;
        width: 100%;
        resize: none;
      }
    </style>
    <title>knowhow - 管理ページ</title>
  </head>
  <body>
    <form runat="server">
      <div class="input-group">
        <h2>管理ページ</h2>
        <p>
          <asp:Label runat="server" id="Label1" Text="SQL"/>
          <asp:RequiredFieldValidator runat="server" id="ReqSql" ControlToValidate="TxtSql" ValidationGroup="input-validation" Display="Dynamic" ForeColor="red" ErrorMessage="（必須入力です）"/>
          <asp:RegularExpressionValidator runat="server" id="RegSql" ControlToValidate="TxtSql" ValidationGroup="input-validation" ValidationExpression="(?s).{1,2500}" Display="Dynamic" ForeColor="red" ErrorMessage="（2,500文字まで）"/>
          <asp:TextBox runat="server" id="TxtSql" Text="SELECT * FROM T_ITEMS ORDER BY NO;" TextMode="MultiLine" Rows=10/>
        </p>
        <p>
          <asp:Button runat="server" id="BtnSend" Text="送信" ValidationGroup="input-validation" OnClick="BtnSend_Click"/>
        </p>
        <p>
          <asp:Label runat="server" id="LblSql" Text=""/>
          <asp:GridView runat="server" id="GrdSql">
            <HeaderStyle BackColor="lightsteelblue"/>
          </asp:GridView>
        </p>
      </div>
    </form>
  </body>
</html>
