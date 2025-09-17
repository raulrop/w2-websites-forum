<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserRegisterComplete.aspx.cs" Inherits="Form_User_UserRegisterComplete" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>会員登録完了</title>
    <link rel="stylesheet" href="~/styles/forms.css" />
</head>
<body>
<div class="form">
    <form id="register_complete_form" runat="server">
        <asp:ScriptManager runat="server" ID="smThreeSeconds" />
        <asp:Timer runat="server" ID="tThreeSeconds" OnTick="tThreeSeconds_Tick" Interval="3000" />
        <div class="title">
            <h1>登録完了しました</h1>
        </div>
        <div class="form-actions">
            <asp:Button runat="server" ID="btnTopPage" CssClass="button" Text="トップページ" OnClick="btnTopPage_Click" />
        </div>
    </form>
</div>
</body>
</html>
