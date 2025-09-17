<%@ Page Title="掲示板Top" MasterPageFile="~/Site.master" Language="C#" AutoEventWireup="true" CodeFile="ForumTop.aspx.cs" Inherits="Form_Forum_ForumTop" %>


<asp:Content id="cBody" ContentPlaceHolderID="MainContent" runat="server">
<!-- Forumの投稿処理 -->
<div class="forum-form">
    <div class="form-post-container">
        <asp:Panel runat="server" ID="pnlForumPost" CssClass="forum-post">
            <asp:Label runat="server" ID="lForumTitle" CssClass="form-label forum-title" Text="投稿フォーム"></asp:Label>
            <br />

            <asp:Label runat="server" id="lErrorMessage" CssClass="error-message" /> <br />

            <asp:Label runat="server" ID="lPoster_Text" CssClass="form-label" Text="投稿者："></asp:Label>
            <asp:Label runat="server" ID="lPoster" CssClass="form-maker"></asp:Label>
            <br />
            <asp:Label runat="server" AssociatedControlID="tbTitle" ID="lTitle" CssClass="form-label" Text="タイトル："></asp:Label>
            <asp:TextBox runat="server" ID="tbTitle" CssClass="form-input" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="tbTitle"
                CssClass="error-message" ErrorMessage="タイトルは必須！" Display="Dynamic" ValidationGroup="vgOne" />
                <asp:CustomValidator runat="server" OnServerValidate="tbTitle_lengthValidate" ClientValidationFunction="tbTitle_lengthClient" 
                    ControlToValidate="tbTitle" ErrorMessage="最大文字数15字" Display="Dynamic" CssClass="error-message" ValidationGroup="vgOne" />
            <asp:Label runat="server" AssociatedControlID="tbBody" ID="lBody" CssClass="form-label--top" Text="本文："></asp:Label>
                <asp:TextBox runat="server" ID="tbBody" CssClass="form-input textarea" TextMode="MultiLine" />
                 <asp:RequiredFieldValidator runat="server" ControlToValidate="tbBody"
                 CssClass="error-message" ErrorMessage="本文は必須！" Display="Dynamic"  ValidationGroup="vgOne" />
                 <asp:CustomValidator runat="server" OnServerValidate="tbBody_lengthValidate" ClientValidationFunction="tbBody_lengthClient" 
                     ControlToValidate="tbBody" ErrorMessage="最大文字数50字" Display="Dynamic" CssClass="error-message" ValidationGroup="vgOne" />
            <div class="form-actions">
            <asp:Button runat="server" ID="btnPost" Text="投稿" OnClick="btnPost_Click" CssClass="button" ValidationGroup="vgOne" />
            </div>
            <br />
            <!-- ページング -->
            <asp:Label ID="lPageNumber" runat="server" Text="ページ " CssClass="page-number-label" Font-Bold="true">
                    <asp:Panel ID="pnlPageNumber" runat="server" ></asp:Panel>
            </asp:Label>
        </asp:Panel>
    </div>
    <!-- 投稿表示のためのGridView -->
    <!-- DataKeyNamesはForumのIDの取得のために利用 --> 
    <div class="grid">
    <asp:GridView ID="gvForumData" DataKeyNames="forum_id" runat="server" OnRowCommand="gvForum_RowCommand" OnRowDataBound="gvForum_RowDataBound" AutoGenerateColumns="False" AllowPaging="False" CssClass="data-table">
        <Columns>
            <asp:TemplateField HeaderText="" ShowHeader="false"> <HeaderStyle CssClass="col-idx"/> <ItemStyle CssClass="col-idx"/> <ItemTemplate> <asp:Label ID="lId" runat="server" Text='<%# Eval("forum_id") %>'/> </ItemTemplate> </asp:TemplateField>
            <asp:TemplateField HeaderText="投稿者"> <HeaderStyle CssClass="col-poster"/> <ItemStyle CssClass="col-poster"/> <ItemTemplate> <asp:Label ID="lPoster" runat="server" Text='<%# Eval("name") %>'/> </ItemTemplate> </asp:TemplateField>
            <asp:TemplateField HeaderText="投稿日時"> <HeaderStyle CssClass="col-date"/> <ItemStyle CssClass="col-date"/> <ItemTemplate> <asp:Label ID="lDate" runat="server" Text='<%# Eval("date_created") %>'/> </ItemTemplate> </asp:TemplateField>
            <asp:TemplateField HeaderText="タイトル"> <HeaderStyle CssClass="col-title"/> <ItemStyle CssClass="col-title"/> <ItemTemplate> <asp:Label ID="lTitle" runat="server" Text='<%# Eval("forum_title") %>'/> </ItemTemplate> </asp:TemplateField>
            <asp:TemplateField HeaderText="本文"> <HeaderStyle CssClass="col-body"/> <ItemStyle CssClass="col-body"/> <ItemTemplate> <asp:Label ID="lBody" runat="server" Text='<%# Eval("forum_text") %>'/> </ItemTemplate> </asp:TemplateField>
            <asp:TemplateField Visible="false"> <ItemTemplate> <asp:Label ID="lUserId" runat="server" Text='<%# Eval("user_id") %>'></asp:Label> </ItemTemplate> </asp:TemplateField>
            <asp:TemplateField HeaderText="" ShowHeader="false" >
                <HeaderStyle CssClass="col-actions" />
                <ItemStyle CssClass="col-actions" />
                <ItemTemplate>
                    <div class="row-actions">
                    <asp:LinkButton runat="server" id="lnkResponse" CssClass="button" CommandName="cResponse" Text="レス" CausesValidation="false" CommandArgument='<%# Eval("forum_id") %>' />
                    <asp:LinkButton runat="server" id="lnkEdit" CssClass="button" CommandName="cEdit" Text="編集" Visible="false" CausesValidation="false" CommandArgument='<%# Eval("forum_id") %>' />
                    <asp:LinkButton runat="server" ID="lnkDelete" CssClass="button" CommandName="cDelete" Text="削除" Visible="false" CausesValidation="false" CommandArgument='<%# Eval("forum_id") %>' />
                    </div>
                    <br />
                    <!-- レスの登録処理 -->
                    <div class="forum-post">
                        <asp:Panel ID="pnlRes" runat="server" Visible="false">
                            <asp:Label runat="server" ID="lResPoster_Text" AssociatedControlID="lResPoster" CssClass="form-label" Text="レス作成者："></asp:Label>
                            <asp:Label runat="server" ID="lResPoster" CssClass="form-maker"></asp:Label>
                            <br />
                            <asp:Label runat="server" ID="lResTitle" Text="レスタイトル：" CssClass="form-label"></asp:Label>
                            <asp:TextBox runat="server" ID="tbResTitle" CssClass="form-input"/>
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="tbResTitle"
                                CssClass="error-message" ErrorMessage="タイトルは必須！" Display="Dynamic" ValidationGroup="vgTwo" />
                                <asp:CustomValidator runat="server" OnServerValidate="tbResTitle_lengthValidate" ClientValidationFunction="tbResTitle_lengthClient" 
                                    ControlToValidate="tbResTitle" ErrorMessage="最大文字数15字" Display="Dynamic" ValidationGroup="vgTwo" CssClass="error-message" />
                            <br />
                            <asp:Label runat="server" CssClass="form-label" ID="lResBody" Text="レス本文：" AssociatedControlID="tbResBody"></asp:Label>
                            <asp:TextBox runat="server" ID="tbResBody" TextMode="MultiLine" CssClass="form-input textarea"/>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="tbResBody"
                            CssClass="error-message" ErrorMessage="本文は必須！" Display="Dynamic" ValidationGroup="vgTwo" />
                            <asp:CustomValidator runat="server" OnServerValidate="tbResBody_lengthValidate" ControlToValidate="tbResBody" 
                                ClientValidationFunction="tbResBody_lengthClient" ErrorMessage="最大文字数50字" Display="Dynamic" ValidationGroup="vgTwo" CssClass="error-message" />
                            <br /> 
                            <div class="form-actions">
                                <asp:LinkButton runat="server" ID="lnkResPost" Text="投稿" CommandName="cResPost" ValidationGroup="vgTwo" CommandArgument='<%# Eval("forum_id") %>' CssClass="button"/>
                                <asp:LinkButton runat="server" ID="lnkResCancel" Text="キャンセル" CommandName="cResCancel" CausesValidation="false" CommandArgument='<%# Eval("forum_id") %>' ValidationGroup="vgTwo" CssClass="button" />
                            </div>
                        </asp:Panel>
                    </div>
                    <!-- レス投稿 -->
                    <asp:Panel id="pnlForumRes" CssClass="res-panel" runat="server" Visible="false" >
                        <asp:GridView ID="gvForumResData" DataKeyNames="forum_res_id" OnRowCommand="gvForumRes_RowCommand" OnRowDataBound="gvForumRes_RowDataBound" runat="server" AutoGenerateColumns="False" CssClass="data-table">
                            <Columns>
                           <asp:TemplateField HeaderText="" ShowHeader="false"> <HeaderStyle CssClass="col-idx"/> <ItemStyle CssClass="col-idx"/> <ItemTemplate> <asp:Label ID="lResId" runat="server" Text='<%# Eval("forum_res_id") %>'/> </ItemTemplate> </asp:TemplateField>
                            <asp:TemplateField HeaderText="レス投稿者"> <HeaderStyle CssClass="col-poster"/> <ItemStyle CssClass="col-poster"/> <ItemTemplate> <asp:Label ID="lResPoster" runat="server" Text='<%# Eval("name") %>'/> </ItemTemplate> </asp:TemplateField>
                            <asp:TemplateField HeaderText="投稿日時"> <HeaderStyle CssClass="col-date"/> <ItemStyle CssClass="col-date"/> <ItemTemplate> <asp:Label ID="lResDate" runat="server" Text='<%# Eval("date_created") %>'/> </ItemTemplate> </asp:TemplateField>
                            <asp:TemplateField HeaderText="タイトル"> <HeaderStyle CssClass="col-title"/> <ItemStyle CssClass="col-title"/> <ItemTemplate> <asp:Label ID="lResTitle" runat="server" Text='<%# Eval("forum_res_title") %>'/> </ItemTemplate> </asp:TemplateField>
                            <asp:TemplateField HeaderText="本文"> <HeaderStyle CssClass="col-body"/> <ItemStyle CssClass="col-body"/> <ItemTemplate> <asp:Label ID="lResBody" runat="server" Text='<%# Eval("forum_res_text") %>'/> </ItemTemplate> </asp:TemplateField>
                            <asp:TemplateField HeaderText="" ShowHeader="false" >
                                <HeaderStyle CssClass="col-actions" />
                                <ItemStyle CssClass="col-actions" />
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" CssClass="button" ID="lnkResEdit" CommandName="cResEdit" Text="編集" Visible="false" CausesValidation="false" CommandArgument='<%# Eval("forum_res_id")%>' />
                                    <asp:LinkButton runat="server" CssClass="button" ID="lnkResDelete" CommandName="cResDelete" Text="削除" Visible="false" CausesValidation="false" />
                                    <div class="forum-post">
                                        <asp:Panel ID="pnlResEdit" CssClass="res-panel" runat="server" Visible="false">
                                            <asp:Label runat="server" ID="lResTitleEdit" AssociatedControlID="tbResTitleEdit" CssClass="form-label" Text="編集レスタイトル："></asp:Label>
                                                <asp:TextBox runat="server" ID="tbResTitleEdit" CssClass="form-input" />
                                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbResTitleEdit"
                                                    CssClass="error-message" ErrorMessage="タイトルは必須！" Display="Dynamic" ValidationGroup="vgFour" />
                                                    <asp:CustomValidator runat="server" OnServerValidate="tbResTitleEdit_lengthValidate" ClientValidationFunction="tbResTitleEdit_lengthClient" 
                                                        ControlToValidate="tbResTitleEdit" ErrorMessage="最大文字数15字" Display="Dynamic" CssClass="error-message" ValidationGroup="vgFour" />
                                            <br />
                                            <asp:Label runat="server" ID="lResBodyEdit" Text="編集レス本文：" AssociatedControlID="tbResBodyEdit" CssClass="form-label"></asp:Label>
                                                <asp:TextBox runat="server" ID="tbResBodyEdit" CssClass="form-input textarea" TextMode="MultiLine"/>
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="tbResBodyEdit"
                                                CssClass="error-message" ErrorMessage="本文は必須！" Display="Dynamic" ValidationGroup="vgFour" />
                                                <asp:CustomValidator runat="server" OnServerValidate="tbResBodyEdit_lengthValidate" ControlToValidate="tbResBodyEdit" 
                                                    ClientValidationFunction="tbResBodyEdit_lengthClient" ErrorMessage="最大文字数50字" Display="Dynamic" CssClass="error-message" ValidationGroup="vgFour" />
                                            <br /> 
                                            <div class="form-actions">
                                                <asp:LinkButton runat="server" ID="lnkResEditPost" CssClass="button" Text="更新" CommandName="cResEditPost" ValidationGroup="vgFour" CommandArgument='<%# Eval("forum_res_id") %>'/>
                                                <asp:LinkButton runat="server" ID="lnkResEditCancel" CssClass="button" Text="キャンセル" CommandName="cResEditCancel" CausesValidation="false" ValidationGroup="vgFour" />
                                            </div>
                                        </asp:Panel>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <br />
                        <!-- レス投稿のページング -->
                        <asp:Label runat="server" AssociatedControlID="rptResPager" ID="lResPager" CssClass="form-label" Text="ページ "></asp:Label>
                        <asp:Repeater ID="rptResPager" runat="server" OnItemCommand="rptResPager_ItemCommand">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkResPage" runat="server"
                                                CommandName="Page"
                                                CommandArgument='<%# Eval("Command") %>'
                                                CausesValidation="false"
                                                Text='<%# Eval("Text") %>'
                                                CssClass="pagination-button" />
                                                &nbsp;
                            </ItemTemplate>
                        </asp:Repeater>
                    </asp:Panel>
                    <!-- 投稿の編集処理 -->
                    <div class="forum-post">
                        <asp:Panel id="pnlEdit" CssClass="res-panel" runat="server" Visible="false">
                        <asp:Label runat="server" ID="lEditTitle_Text" CssClass="form-label" Text="編集タイトル："></asp:Label>
                        <asp:TextBox runat="server" ID="tbEditTitle" CssClass="form-input" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="tbEditTitle"
                            CssClass="error-message" ErrorMessage="タイトルは必須！" Display="Dynamic" ValidationGroup="vgThree" />
                            <asp:CustomValidator runat="server" OnServerValidate="tbEditTitle_lengthValidate" ClientValidationFunction="tbEditTitle_lengthClient" 
                                ControlToValidate="tbEditTitle" CssClass="error-message" ErrorMessage="最大文字数15字" Display="Dynamic" ValidationGroup="vgThree" />
                        <br />
                        <asp:Label runat="server" ID="lEditBody_Text" Text="編集本文：" CssClass="form-label--top"></asp:Label>
                        <asp:TextBox runat="server" ID="tbEditBody" CssClass="form-input textarea"  TextMode="MultiLine"/>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="tbEditBody"
                        CssClass="error-message" ErrorMessage="本文は必須！" Display="Dynamic" ValidationGroup="vgThree" />
                        <asp:CustomValidator runat="server" OnServerValidate="tbEditBody_lengthValidate" ControlToValidate="tbEditBody" 
                            ClientValidationFunction="tbEditBody_lengthClient" CssClass="error-message" ErrorMessage="最大文字数50字" Display="Dynamic" ValidationGroup="vgThree" />
                        <br /> 
                        <div class="form-actions">
                            <asp:LinkButton runat="server" ID="btnEdit" CssClass="button" Text="更新" CommandName="cEditPost" CommandArgument='<%# Eval("forum_id") %>' ValidationGroup="vgThree"/>
                            <asp:LinkButton runat="server" ID="btnEditCancel" CssClass="button" Text="キャンセル" CommandName="cEditCancel" CausesValidation="false"/>
                        </div>
                    </asp:Panel>
                </div>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    </div> <!-- grid -->
</div> <!-- post-form -->
<br />

<!-- 最大限の認証のためJavaScript使用 -->
<script type="text/javascript">

    function tbTitle_lengthClient(sender, args) {
        try {
            var value = args.Value;
            args.IsValid = (value.length <= 15);
        } catch (e) {
            args.IsValid = false;
        }
    }

    function tbBody_lengthClient(sender, args) {
        try {
            var value = args.Value;
            args.IsValid = (value.length <= 15);
        } catch (e) {
            args.IsValid = false;
        }
    }

    function tbResTitle_lengthClient(sender, args) {
        try {
            var value = args.Value;
            args.IsValid = (value.length <= 15);
        } catch (e) {
            args.IsValid = false;
        }
    }

    function tbResBody_lengthClient(sender, args) {
        try {
            var value = args.Value;
            args.IsValid = (value.length <= 50);
        } catch (e) {
            args.IsValid = false;
        }
    }

    function tbEditTitle_lengthClient(sender, args) {
        try {
            var value = args.Value;
            args.IsValid = (value.length <= 15);
        } catch (e) {
            args.IsValid = false;
        }
    }

    function tbEditBody_lengthClient(sender, args) {
        try {
            var value = args.Value;
            args.IsValid = (value.length <= 50);
        } catch (e) {
            args.IsValid = false;
        }
    }

    function tbResTitleEdit_lengthClient(sender, args) {
        try {
            var value = args.Value;
            args.IsValid = (value.length <= 15);
        } catch (e) {
            args.IsValid = false;
        }
    }

    function tbResBodyEdit_lengthClient(sender, args) {
        try {
            var value = args.Value;
            args.IsValid = (value.length <= 50);
        } catch (e) {
            args.IsValid = false;
        }
    }
</script>

</asp:Content>

