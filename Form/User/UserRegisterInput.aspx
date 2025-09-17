<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserRegisterInput.aspx.cs" Inherits="Form_User_UserRegisterInput" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>アカウント作成</title>
    <link rel="stylesheet" href="~/styles/forms.css"/>
</head>
<body>
<div class="top">
    <h1 class="title">アカウント作成</h1>
    <br />
    <div class="form">
        <form id="register_form" runat="server">

        <asp:Label runat="server" ID="lErrorMessage" CssClass="error-message"></asp:Label>

        <div>
            <asp:TextBox runat="server" ID="tbUserName" CssClass="form-input" placeholder="ユーザー名(50字以内)"/>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="tbUserName"
                CssClass="error-message" ErrorMessage="ユーザー名は必須！" Display="Dynamic" />
            <asp:CustomValidator runat="server" OnServerValidate="tbUserName_lengthValidate" ClientValidationFunction="tbUserName_lengthClient" ControlToValidate="tbUserName" CssClass="error-message" Display="Dynamic"  ErrorMessage="最大文字数50字" />
        </div>
        <br />
        <div >
            <asp:TextBox runat="server" ID="tbLoginId" CssClass="form-input" placeholder="ログインID(15字以内)" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="tbLoginId"
            CssClass="error-message" ErrorMessage="ログインIDは必須！" Display="Dynamic"/>
            <asp:CustomValidator runat="server" OnServerValidate="tbLoginId_lengthValidate" ClientValidationFunction="tbLoginId_lengthClient" ControlToValidate="tbLoginId" CssClass="error-message" ErrorMessage="最小3字・最大15字" Display="Dynamic" />
        </div>
        <br />
        <div>
            <asp:TextBox runat="server" ID="tbPassword" TextMode="Password" CssClass="form-input" placeholder="パスワード(15字以内)" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="tbPassword"
                CssClass="error-message" ErrorMessage="パスワードは必須！" Display="Dynamic" />
            <asp:CustomValidator runat="server" OnServerValidate="tbPassword_lengthValidate" ClientValidationFunction="tbPassword_lengthClient" ControlToValidate="tbPassword" CssClass="error-message" ErrorMessage="最小6字・最大15字" Display="Dynamic" />
            <!-- 入力は半角英数字のみ -->
            <asp:RegularExpressionValidator runat="server" CssClass="error-message" ControlToValidate="tbPassword" ValidationExpression="^[a-zA-Z0-9]*$" ErrorMessage="半角英数字のみを入力してください。" Display="Dynamic" />
        </div>
        <br />
        <div class="form-actions">
            <asp:Button runat="server" CssClass="button" ID="btnUserRegister" OnClick="btnUserRegister_Click" Text="確認する" />
            <asp:Button runat="server" CssClass="button" ID="btnReturn" UseSubmitBehavior="False" CausesValidation="False" Text="戻る" OnClick="btnReturn_Click" />
        </div>
        </form>
    </div>
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
