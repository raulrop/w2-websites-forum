<%@ Page Title="掲示板" Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><%: Page.Title %></title>
    <link rel="stylesheet" href="~/styles/forms.css" />
</head>
<body>
<div class="top">
    <h1 class="title">掲示板</h1>
    <br />

    <form id="user_modify_form" runat="server">

        <asp:Label runat="server" ID="lErrorMessage" CssClass="error-message"></asp:Label>
        <br />
        <div class="form">

            <!-- ログインIDのTextBox-->
            <asp:TextBox runat="server" ID="tbLoginId" placeholder="ログインID" CssClass="form-input" />
            <asp:RequiredFieldValidator runat="server" CssClass="error-message" ControlToValidate="tbLoginId" ErrorMessage="ログインIDを入力してください。" Display="Dynamic" />
            <asp:CustomValidator runat="server" OnServerValidate="tbLoginId_lengthValidate" ClientValidationFunction="tbLoginId_lengthClient" ControlToValidate="tbLoginId" ErrorMessage="ログインIDは最小3字・15字以内" Display="Dynamic" CssClass="error-message" />
            <br />
            <!-- パスワードのTextBox -->
            <asp:TextBox runat="server" ID="tbPassword" placeholder="パスワード" TextMode="Password" CssClass="form-input"/>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="tbPassword" ErrorMessage="パスワードを入力してください。" Display="Dynamic" CssClass="error-message" />
            <asp:CustomValidator runat="server" OnServerValidate="tbPassword_lengthValidate" ClientValidationFunction="tbPassword_lengthClient" CssClass="error-message" ControlToValidate="tbPassword" ErrorMessage="パスワードは最小6字・15字以内" Display="Dynamic" />
            <!-- ログインボタン -->
            <div class="form-actions">
                <asp:Button runat="server" ID="btnLogin" Text="ログイン" OnClick="btnLogin_Click" CssClass="button" />
            
            <br />
            <p>
                アカウントを持っていない方は<asp:HyperLink runat="server" ID="hlToRegister" Text="アカウントを作成する" CssClass="hyperlink" NavigateUrl="~/Form/User/UserRegisterInput.aspx" />
            </p>
            </div>
        </div>
    </form>
</div>
<script>
    // クライアント側でも認証する関数

    function tbUserName_lengthClient(sender, args) {
        try {
            var value = args.Value;
            args.IsValid = (value.length <= 50);
        } catch (e) {
            args.IsValid = false;
        }
    }

    function tbLoginId_lengthClient(sender, args) {
        try {
            var value = args.Value;
            args.IsValid = (value.length >= 3 && value.length <= 15);
        } catch (e) {
            args.IsValid = false;
        }
    }

    function tbPassword_lengthClient(sender, args) {
        try {
            var value = args.Value;
            args.IsValid = (value.length >= 6 && value.length <= 15);
        } catch (e) {
            args.IsValid = false;
        }
    }
</script>

</body>
</html>
