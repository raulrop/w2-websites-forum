<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserModifyInput.aspx.cs" Inherits="Form_User_UserModifyInput" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>会員情報変更</title>
    <link rel="stylesheet" href="~/styles/forms.css" />
</head>
<body>
<div class="top">
    <h1 class="title">会員情報変更</h1>
    <br />
    <h3 class="form-label">変更したい情報を入力してください</h3>

    <form id="user_modify_form" runat="server">

    <asp:Label runat="server" ID="lErrorMessage" CssClass="error-message"></asp:Label>

    <div class="form">
            <asp:Label runat="server" ID="lUserName" CssClass="form-label" Text="新しいユーザー名："></asp:Label>
            <asp:TextBox runat="server" ID="tbUserName" CssClass="form-input" placeholder="50字以内" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="tbUserName"
                CssClass="error-message" ErrorMessage="ユーザー名は必須！" Display="Dynamic" />
            <asp:CustomValidator runat="server" OnServerValidate="tbUserName_lengthValidate" ControlToValidate="tbUserName" CssClass="error-message" ErrorMessage="最大文字数50字" Display="Dynamic"/> 
    <br />
        <asp:Label runat="server" ID="lLoginId" CssClass="form-label" Text="新しいログインID："></asp:Label>
        <asp:TextBox runat="server" ID="tbLoginId" CssClass="form-input" placeholder="15字以内" />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="tbLoginId"
        CssClass="error-message" ErrorMessage="ログインIDは必須！" Display="Dynamic"/>
        <asp:CustomValidator runat="server" OnServerValidate="tbLoginId_lengthValidate" ControlToValidate="tbLoginId" CssClass="error-message" ErrorMessage="最小3字・最大15字"  Display="Dynamic"/>
    <br />
        <asp:Label runat="server" ID="lPassword" CssClass="form-label" Text="新しいパスワード："></asp:Label>
        <asp:TextBox runat="server" ID="tbPassword" TextMode="Password" CssClass="form-input" placeholder="15字以内" />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="tbPassword" 
            CssClass="error-message" ErrorMessage="パスワードは必須！" Display="Dynamic" />
        <asp:CustomValidator runat="server" OnServerValidate="tbPassword_lengthValidate" Display="Dynamic" ControlToValidate="tbPassword" CssClass="error-message" ErrorMessage="最小6字・最大15字" />
        <!-- 入力は半角英数字のみ -->
        <asp:RegularExpressionValidator runat="server" CssClass="error-message" ControlToValidate="tbPassword" ValidationExpression="^[a-zA-Z0-9]*$" ErrorMessage="半角英数字のみを入力してください。" Display="Dynamic" />
        <div class="form-actions">
            <asp:Button runat="server" ID="btnUserRegister" OnClick="btnUserModify_Click" CssClass="button" Text="確認する" />
            <asp:Button runat="server" ID="btnTopPage" UseSubmitBehavior="False" CausesValidation="False" Text="戻る" CssClass="button" OnClick="btnTopPage_Click" />
        </div>
    </div>
    </form>
</div>
</body>
</html>
