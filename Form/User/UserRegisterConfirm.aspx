<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserRegisterConfirm.aspx.cs" Inherits="Form_User_UserRegisterConfirm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>アカウント作成の確認</title>
    <link rel="stylesheet" href="~/styles/forms.css"  />
</head>
<body>
<div class="form">
    <form id="register_confirm_form" runat="server">
    <div class="form-title">
        <h1>
            <asp:Label runat="server" ID="lTitle" CssClass="title"></asp:Label>
        </h1>
        <br />
    </div>
    <div class="error-message">
        <asp:Label runat="server" CssClass="error-message" ID="lErrorMessage"></asp:Label>
        <br />
    </div>
    <!-- ユーザ名のLabel-->
    <asp:Label runat="server" ID="lUserName_Text" CssClass="form-label--text" AssociatedControlID="lUserName" Text="ユーザ名："></asp:Label>
    <asp:Label runat="server" id="lUserName" CssClass="form-label"></asp:Label> 
    <br />
    <!-- ログインIDのLabel -->
    <asp:Label runat="server" ID="lLoginId_Text" CssClass="form-label--text" Text="ログインID：" AssociatedControlID="lLoginId"></asp:Label>
    <asp:Label runat="server" ID="lLoginId" CssClass="form-label"></asp:Label>
    <br />
    <!-- パスワードのLabel -->
    <asp:Label runat="server" ID="lPassword_Text" CssClass="form-label--text" Text="パスワード：" AssociatedControlID="lPassword"></asp:Label>
    <asp:Label runat="server" ID="lPassword" CssClass="form-label"></asp:Label>
    <br />
    <div class="form-actions">
        <!-- ログインボタン -->
        <asp:Button runat="server" ID="btnUserRegisterConfirm" Text="登録" OnClick="btnUserRegisterConfirm_Click" CssClass="button" />
        <asp:Button runat="server" ID="btnReturn" UseSubmitBehavior="false" OnClick="btnReturn_Click" Text="戻る" CssClass="button" />
        <br />
    </div>
    </form>
</div>
</body>
</html>
