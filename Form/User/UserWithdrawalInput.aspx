<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserWithdrawalInput.aspx.cs" Inherits="Form_User_UserWithdrawalInput" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>会員退会</title>
    <link rel="stylesheet" href="~/styles/forms.css" />
</head>
<body>
<div class="form">
    <div class="title">
        <h1>アカウント削除</h1>
    </div>
    <form id="user_withdrawal_form" runat="server">
        <div class="error-message">
            <asp:Literal runat="server" id="lErrorMessage" />
        </div>
        <div class="form-actions">
            <asp:Button runat="server" CssClass="button" ID="btnDelete" OnClick="btnDelete_Click" Text="削除" />
            <asp:Button runat="server" CssClass="button" ID="btnCancel" Text="キャンセル" UseSubmitBehavior="false" OnClick="btnCancel_Click" />
        </div>
    </form>
</div>
</body>
</html>
