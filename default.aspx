<%@ Page Language="C#" CodeFile="default.aspx.cs" Inherits="Knowhow" Debug="true" %>
<!DOCTYPE html>
<html lang="ja">
  <head>
    <meta charset="UTF-8">
    <meta http-equiv="x-ua-compatible" content="IE=Edge">
    <style type="text/css">
      body
      {
        background: white;
        margin: 0;
        padding: 0;
      }
      .main
      {
        box-sizing: border-box;
        margin-left: 240px;
        padding: 20px 40px;
      }
      .menu
      {
        box-sizing: border-box;
        top: 0px;
        height: 100%;
        width: 240px;
        position: fixed;
        overflow: auto;
        background: lightsteelblue;
        padding: 20px 20px;
        word-wrap: break-word;
      }
      .menu input[type="text"]
      {
        box-sizing: border-box;
        width: 100%;
      }
      .menu input[type="submit"]
      {
        box-sizing: border-box;
      }
      .info
      {
        color: gray;
        text-align: right;
      }
      .input-group input[type="text"]
      {
        box-sizing: border-box;
        width: 100%;
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
      .admin
      {
        font-size: small;
        text-align: right;
      }
    </style>
    <script runat="server">
      private string SetLink(string mes)
      {
        mes = System.Text.RegularExpressions.Regex.Replace(mes, @"(https?|HTTPS?)://[a-zA-Z_0-9\-\.~#\$&\+\/:;=\?%]+", "<a href=\"$&\" target=\"_blank\" rel=\"noopener noreferrer\">$&</a>");
        mes = System.Text.RegularExpressions.Regex.Replace(mes, @"[a-zA-Z_0-9\-\.]+@[a-zA-Z_0-9\-\.]+", "<a href=mailto:$&>$&</a>");
        return mes;
      }
    </script>
    <title>knowhow</title>
  </head>
  <body>
    <form runat="server">
      <div class="menu">
        <h2>knowhow</h2>
        <ul>
          <asp:Repeater runat="server" id="RepMenu">
            <ItemTemplate>
              <li><asp:LinkButton runat="server" id="LbtMenu" Text='<%# HttpUtility.HtmlEncode(Eval("CATEGORY"))%>' CommandName='<%# Eval("CATEGORY")%>' OnCommand="LbtMenu_Click"/></li>
            </ItemTemplate>
          </asp:Repeater>
        </ul>
        <hr>
        <p>
          <asp:TextBox runat="server" id="TxtSearch" Text="" MaxLength="100"/>
        </p>
        <p>
          <asp:Button runat="server" id="BtnSearch" Text="検索" ValidationGroup="search-validation" OnClick="BtnSearch_Click"/>
          <asp:RequiredFieldValidator runat="server" id="ReqSearch" ControlToValidate="TxtSearch" ValidationGroup="search-validation" Display="Dynamic" ForeColor="red" ErrorMessage="（必須入力です）"/>
          <asp:RegularExpressionValidator runat="server" id="RegSearch" ControlToValidate="TxtSearch" ValidationGroup="search-validation" ValidationExpression="(?s).{1,100}" Display="Dynamic" ForeColor="red" ErrorMessage="（100文字まで）"/>
        </p>
      </div>
      <div class="main">
        <h2><asp:Label runat="server" id="LblResult" Text=""/></h2>
        <hr>
        <asp:Repeater runat="server" id="RepMain">
          <ItemTemplate>
            <h3><asp:Label runat="server" id="LblTitle" Text='<%# HttpUtility.HtmlEncode(Eval("TITLE"))%>'/></h3>
            <p>
              <asp:Label runat="server" id="LblMessage" Text='<%# SetLink(HttpUtility.HtmlEncode(Eval("MESSAGE")).Replace("\n", "<br/>"))%>'/>
            </p>
            <div class="info">
              <p>
                <asp:Label runat="server" id="LblTimestamp" Text='<%# HttpUtility.HtmlEncode(Eval("TS"))%>'/>
                [<asp:Label runat="server" id="LblCategory" Text='<%# HttpUtility.HtmlEncode(Eval("CATEGORY"))%>'/>]
                <asp:Label runat="server" id="LblAuthor" Text='<%# HttpUtility.HtmlEncode(Eval("AUTHOR"))%>'/>
                (<asp:Label runat="server" id="LblIp" Text='<%# HttpUtility.HtmlEncode(Eval("IP"))%>'/>)
                <asp:LinkButton runat="server" id="LbtUpdate" Text="変更" CommandName='<%# Eval("NO")%>' CommandArgument='<%# Eval("TS")%>' OnCommand="LbtUpdate_Click"/>
                <asp:LinkButton runat="server" id="LbtDelete" Text="削除" CommandName='<%# Eval("NO")%>' CommandArgument='<%# Eval("TS")%>' OnClientClick='return confirm("削除してもよろしいですか？");' OnCommand="LbtDelete_Click"/>
              </p>
            </div>
            <hr>
          </ItemTemplate>
        </asp:Repeater>
        <div class="input-group">
          <h3>入力フォーム</h3>
          <p>
            <asp:Label runat="server" id="Label1" Text="カテゴリ"/>
            <asp:RequiredFieldValidator runat="server" id="ReqCategory" ControlToValidate="TxtCategory" ValidationGroup="input-validation" Display="Dynamic" ForeColor="red" ErrorMessage="（必須入力です）"/>
            <asp:RegularExpressionValidator runat="server" id="RegCategory" ControlToValidate="TxtCategory" ValidationGroup="input-validation" ValidationExpression="(?s).{1,100}" Display="Dynamic" ForeColor="red" ErrorMessage="（100文字まで）"/>
            <asp:TextBox runat="server" id="TxtCategory" Text="" MaxLength="100"/>
          </p>
          <p>
            <asp:Label runat="server" id="Label2" Text="タイトル"/>
            <asp:RequiredFieldValidator runat="server" id="ReqTitle" ControlToValidate="TxtTitle" ValidationGroup="input-validation" Display="Dynamic" ForeColor="red" ErrorMessage="（必須入力です）"/>
            <asp:RegularExpressionValidator runat="server" id="RegTitle" ControlToValidate="TxtTitle" ValidationGroup="input-validation" ValidationExpression="(?s).{1,100}" Display="Dynamic" ForeColor="red" ErrorMessage="（100文字まで）"/>
            <asp:TextBox runat="server" id="TxtTitle" Text="" MaxLength="100"/>
          </p>
          <p>
            <asp:Label runat="server" id="Label3" Text="投稿者"/>
            <asp:RequiredFieldValidator runat="server" id="ReqAuthor" ControlToValidate="TxtAuthor" ValidationGroup="input-validation" Display="Dynamic" ForeColor="red" ErrorMessage="（必須入力です）"/>
            <asp:RegularExpressionValidator runat="server" id="RegAuthor" ControlToValidate="TxtAuthor" ValidationGroup="input-validation" ValidationExpression="(?s).{1,100}" Display="Dynamic" ForeColor="red" ErrorMessage="（100文字まで）"/>
            <asp:TextBox runat="server" id="TxtAuthor" Text="" MaxLength="100"/>
          </p>
          <p>
            <asp:Label runat="server" id="Label4" Text="本文"/>
            <asp:RequiredFieldValidator runat="server" id="ReqMessage" ControlToValidate="TxtMessage" ValidationGroup="input-validation" Display="Dynamic" ForeColor="red" ErrorMessage="（必須入力です）"/>
            <asp:RegularExpressionValidator runat="server" id="RegMessage" ControlToValidate="TxtMessage" ValidationGroup="input-validation" ValidationExpression="(?s).{1,1000}" Display="Dynamic" ForeColor="red" ErrorMessage="（1,000文字まで）"/>
            <asp:TextBox runat="server" id="TxtMessage" Text="" TextMode="MultiLine" Rows=8/>
          </p>
          <p>
            <asp:Button runat="server" id="BtnSend" Text="送信" ValidationGroup="input-validation" CommandName="add" OnClick="BtnSend_Click"/>
            <asp:Button runat="server" id="BtnCancel" Text="キャンセル" OnClick="BtnCancel_Click"/>
          </p>
        </div>
        <div class="admin">
          <a href="admin.aspx" target="_blank" rel="noopener noreferrer">管理ページ</a>
        </div>
      </div>
    </form>
  </body>
</html>
