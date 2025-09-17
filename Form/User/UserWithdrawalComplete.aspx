<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserWithdrawalComplete.aspx.cs" Inherits="Form_User_UserWithdrawalComplete" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>会員退会完了</title>
    <link rel="stylesheet" href="~/styles/forms.css" />
</head>
<body>
    <div class="form">
    <form id="user_withdrawal_complete" runat="server">
        <asp:ScriptManager runat="server" ID="smThreeSeconds" />
        <asp:Timer runat="server" ID="tThreeSeconds" OnTick="tThreeSeconds_Tick" Interval="3000" />
        <div class="title">
            <h2>アカウント削除処理が完了しました</h2>
        </div>
        <div class="form-actions">
            <asp:Button runat="server" ID="btnTopPage" OnClick="btnTopPage_Click" CssClass="button" Text="トップへ" />
        </div>
    </form>
    </div>
</body>
</html>
