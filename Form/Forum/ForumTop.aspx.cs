using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.Common.Web;
using w2.Common.Sql;
using System.Data;
using System.Configuration;
using System.Web.Services.Description;
using System.IO;
using System.Web.DynamicData;
using System.Runtime.CompilerServices;

public partial class Form_Forum_ForumTop : System.Web.UI.Page
{
	// ページング処理

	const int PAGE_SIZE = 10;
	const int RES_PAGE_SIZE = 3;
	public int current_page
	{
		get { return ViewState["CurrentPage"] == null ? 1 : (int)ViewState["CurrentPage"]; }
		set { ViewState["CurrentPage"] = value; }
	}
	public int current_res_page
	{
		get { return ViewState["CurrentResPage"] == null ? 0 : (int)ViewState["CurrentResPage"]; }
		set { ViewState["CurrentResPage"] = value; }
	}

	// レス・レス編集のPanelの表示のため
	public int expanded_forum_id
	{
		get { return ViewState["ExpandedForumId"] == null ? 0 : (int)ViewState["ExpandedForumId"]; }
		set { ViewState["ExpandedForumId"] = value; }
	}

	protected void Page_PreRender(object sender, EventArgs e)
	{
		lnkPageNumber_SetActive();
	}

	protected void Page_Init(object sender, EventArgs e)
	{
		var ht = (Hashtable)Session["param"];
		if (expanded_forum_id > 0)
		{
			Forum_BindData();

			// DataKeyを通し拡張した列の取得
			for (int i = 0; i < gvForumData.Rows.Count; i++)
			{
				// DataKeyNamesとしてforum_idの取得
				int key = (int)gvForumData.DataKeys[i].Value;
				if (key == expanded_forum_id)
				{
					GridViewRow row = gvForumData.Rows[i];
					Label lPoster = (Label)row.FindControl("lPoster");
					if (lPoster != null) lPoster.Text = HtmlSanitizer.HtmlEncode((String)ht["name"]);
					ForumRes_BindData(row, expanded_forum_id, current_res_page, true);
					break;
				}
			}
		}
	}

	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
		// ポストバックの代わりにOnInitごとに呼び出す
		lnkPageNumber_Add();
	}
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			lErrorMessage.Text = "";
			Forum_BindData();
		}
		var ht = (Hashtable)Session["param"];
		// 未ログインのエラー
		if (ht == null)
		{
			Response.Redirect("~/Default.aspx");
			return;
		}
		var data = (DataRowView)Get_Forum_Count();
		if ((int)data[0] == 0) { lErrorMessage.Text = HtmlSanitizer.HtmlEncode("掲示板の項目はありません。"); }
		else { lErrorMessage.Text = ""; }
		lPoster.Text = HtmlSanitizer.HtmlEncode(ht["name"].ToString());
	}

	protected void btnPost_Click(object sender, EventArgs e)
	{
		var ht = (Hashtable)Session["param"];
		var result = Forum_Insert((int)ht["user_id"], tbTitle.Text, tbBody.Text);
		if (result)
		{
			tbTitle.Text = "";
			tbBody.Text = "";
			Forum_BindData();
		}
		else
		{
			lErrorMessage.Text = HtmlSanitizer.HtmlEncode("投稿は失敗しました");
		}
	}

	// **Forum**
	// レス・編集・削除のボタン処理
	protected void gvForum_RowCommand(object sender, GridViewCommandEventArgs e)
	{
		var ht = (Hashtable)Session["param"];

		// レス表示ボタン処理
		if (e.CommandName == "cResponse")
		{
			var source = (Control)e.CommandSource;
			var row = (GridViewRow)source.NamingContainer;

			Label lResPoster = (Label)row.FindControl("lResPoster");
			Panel pnlForumRes = (Panel)row.FindControl("pnlForumRes");
			Panel pnlRes = (Panel)row.FindControl("pnlRes");
			GridView gvForumResData = (GridView)row.FindControl("gvForumResData");
			TextBox tbResTitle = (TextBox)row.FindControl("tbResTitle");
			TextBox tbResBody = (TextBox)row.FindControl("tbResBody");

			Panel pnlEdit = (Panel)row.FindControl("pnlEdit");

			if (pnlRes != null && pnlEdit != null && lResPoster != null && pnlForumRes != null && gvForumResData != null && tbResTitle != null && tbResBody != null) 
			{
				if (pnlForumRes.Visible || pnlRes.Visible)
				{   // 表示の状態ならまた押したら非表示にする
					tbResTitle.Text = "";
					tbResBody.Text = "";
					// 他の列は非表示
					foreach (GridViewRow r in gvForumData.Rows)
					{
						TextBox tbResTitle_Other = (TextBox)r.FindControl("tbResTitle");
						if (tbResTitle_Other != null) tbResTitle_Other.Text = "";
						TextBox tbResBody_Other = (TextBox)r.FindControl("tbResBody");
						if (tbResBody_Other != null) tbResBody_Other.Text = "";
					}
					pnlForumRes.Visible = false;
					pnlRes.Visible = false;

					expanded_forum_id = 0;
				}
				else
				{
					pnlEdit.Visible = false;    // 編集の非表示
					lResPoster.Text = HtmlSanitizer.HtmlEncode(ht["name"].ToString());


					// 他の列は非表示
					foreach (GridViewRow r in gvForumData.Rows)
					{
						TextBox tbResTitle_Other = (TextBox)r.FindControl("tbResTitle");
						if (tbResTitle_Other != null) tbResTitle_Other.Text = "";
						TextBox tbResBody_Other = (TextBox)r.FindControl("tbResBody");
						if (tbResBody_Other != null) tbResBody_Other.Text = "";
						Panel pnlRes_Other = (Panel)r.FindControl("pnlRes");
						if (pnlRes_Other != null) pnlRes_Other.Visible = false;
						Panel pnlForumRes_Other = (Panel)r.FindControl("pnlForumRes");
						if (pnlForumRes_Other != null) pnlForumRes_Other.Visible = false;
						Panel pnlEdit_Other = (Panel)r.FindControl("pnlEdit");
						if (pnlEdit_Other != null) pnlEdit_Other.Visible = false;
					}

					// forum_idをParseしてから取得
					// (e.g. '#010' => 10)
					var commandArg = (string)e.CommandArgument;
					string caNumericPart = commandArg.TrimStart('#');
					int forum_id = int.Parse(caNumericPart);

					//レスページング処理
					expanded_forum_id = forum_id;
					current_res_page = 0;

					gvForumResData.DataSource = Get_ForumRes((int)forum_id);
					gvForumResData.DataBind();

					var data = Get_ForumRes_Count((int)forum_id);

					// この列のレスは無ならページング非表示
					if ((int)data[0] != 0)
					{
						ForumRes_BindPager(row, forum_id);
						pnlForumRes.Visible = true;
					}


					pnlRes.Visible = true;
				}
			}
		}
		// レス登録ボタン処理
		else if (e.CommandName == "cResPost")
		{
			var source = (Control)e.CommandSource;
			var row = (GridViewRow)source.NamingContainer;

			var commandArg = (string)e.CommandArgument;
			string caNumericPart = commandArg.TrimStart('#');
			int forum_id = int.Parse(caNumericPart);

			Panel pnlRes = (Panel)row.FindControl("pnlRes");
			Label lResPoster = (Label)row.FindControl("lResPoster");
			TextBox tbResTitle = (TextBox)row.FindControl("tbResTitle");
			TextBox tbResBody = (TextBox)row.FindControl("tbResBody");

			if (tbResTitle != null && tbResBody != null && pnlRes != null && lResPoster != null)
			{
				var result = ForumRes_Insert((int)forum_id, (int)ht["user_id"], tbResTitle.Text, tbResBody.Text);
				if (result)
				{
					tbResTitle.Text = "";
					tbResBody.Text = "";

					// レスのBindData
					RebindParentAndShowChild(forum_id);
				}
			}
		}
		// レス投稿のキャンセル処理
		else if (e.CommandName == "cResCancel")
		{
			var source = (Control)e.CommandSource;
			var row = (GridViewRow)source.NamingContainer;

			Panel pnlRes = (Panel)row.FindControl("pnlRes");
			Panel pnlForumRes = (Panel)row.FindControl("pnlForumRes");
			TextBox tbResTitle = (TextBox)row.FindControl("tbResTitle");
			TextBox tbResBody = (TextBox)row.FindControl("tbResBody");

			if (pnlForumRes != null && tbResTitle != null && tbResBody != null)
			{
				// forum_idをParseしてから取得
				// (e.g. '#010' => 10)
				var commandArg = (string)e.CommandArgument;
				string caNumericPart = commandArg.TrimStart('#');
				int forum_id = int.Parse(caNumericPart);

				var data = Get_ForumRes_Count((int)forum_id);

				tbResTitle.Text = "";
				tbResBody.Text = "";
				pnlRes.Visible = false;
				if ((int)data[0] == 0)
				{
					pnlForumRes.Visible = false;
				}
			}
		}
		// 編集表示ボタン処理
		else if (e.CommandName == "cEdit")
		{
			var source = (Control)e.CommandSource;
			var row = (GridViewRow)source.NamingContainer;

			Panel pnlEdit = (Panel)row.FindControl("pnlEdit");
			Panel pnlRes = (Panel)row.FindControl("pnlRes");
			Panel pnlForumRes = (Panel)row.FindControl("pnlForumRes");
			TextBox tbEditTitle = (TextBox)row.FindControl("tbEditTitle");
			TextBox tbEditBody = (TextBox)row.FindControl("tbEditBody");

			if (pnlEdit != null && pnlRes != null && pnlForumRes != null && tbEditTitle != null && tbEditBody != null)
			{
				if (pnlEdit.Visible)
				{   // キャンセルと同様
					tbEditTitle.Text = "";
					tbEditBody.Text = "";
					pnlEdit.Visible = false;
				}
				else
				{
					//レスの非表示
					pnlRes.Visible = false;
					pnlForumRes.Visible = false;

					// 他の列は非表示
					foreach (GridViewRow r in gvForumData.Rows)
					{
						Panel pnlRes_Other = (Panel)r.FindControl("pnlRes");
						if (pnlRes_Other != null) pnlRes_Other.Visible = false;
						Panel pnlForumRes_Other = (Panel)r.FindControl("pnlForumRes");
						if (pnlForumRes_Other != null) pnlForumRes_Other.Visible = false;
						Panel pnlEdit_Other = (Panel)r.FindControl("pnlEdit");
						if (pnlEdit_Other != null) pnlEdit_Other.Visible = false;
					}

					// forum_idをParseしてから取得
					// (e.g. '#010' => 10)
					var commandArg = (string)e.CommandArgument;
					string caNumericPart = commandArg.TrimStart('#');
					int forum_id = int.Parse(caNumericPart);

					// 他の列は非表示
					foreach (GridViewRow r in gvForumData.Rows)
					{
						Panel pnlEdit_Other = (Panel)r.FindControl("pnlEdit");
						if (pnlEdit_Other != null) pnlEdit_Other.Visible = false;
					}

					var data = Get_ForumById((int)forum_id);

					tbEditTitle.Attributes["placeholder"] = data["forum_title"].ToString();
					tbEditBody.Attributes["placeholder"] = data["forum_text"].ToString();

					pnlEdit.Visible = true;
				}
			}
		}
		// 編集ボタン処理
		else if (e.CommandName == "cEditPost")
		{
			var source = (Control)e.CommandSource;
			var row = (GridViewRow)source.NamingContainer;

			// CommandArgumentとしてforum_idの取得
			var commandArg = (string)e.CommandArgument;
			string caNumericPart = commandArg.TrimStart('#');
			int forum_id = int.Parse(caNumericPart);

			TextBox tbEditTitle = (TextBox)row.FindControl("tbEditTitle");
			TextBox tbEditBody = (TextBox)row.FindControl("tbEditBody");

			if (tbEditTitle != null && tbEditBody != null)
			{
				var result = Forum_Update((int)forum_id, (string)tbEditTitle.Text, (string)tbEditBody.Text);
				if (result) Forum_BindData();
			}
		}
		// 編集キャンセルボタン処理
		else if (e.CommandName == "cEditCancel")
		{
			var source = (Control)e.CommandSource;
			var row = (GridViewRow)source.NamingContainer;

			Panel pnlEdit = (Panel)row.FindControl("pnlEdit");
			TextBox tbEditTitle = (TextBox)row.FindControl("tbEditTitle");
			TextBox tbEditBody = (TextBox)row.FindControl("tbEditBody");

			if (pnlEdit != null && tbEditTitle != null && tbEditBody != null)
			{
				tbEditTitle.Text = "";
				tbEditBody.Text = "";
				pnlEdit.Visible = false;
			}
		}
		// 削除ボタン処理
		else if (e.CommandName == "cDelete")
		{
			var source = (Control)e.CommandSource;
			var row = (GridViewRow)source.NamingContainer;

			// CommandArgumentとしてforum_idの取得
			var commandArg = (string)e.CommandArgument;
			string caNumericPart = commandArg.TrimStart('#');
			int forum_id = int.Parse(caNumericPart);

			var result = Forum_Delete((int)forum_id);
			if (result) Forum_BindData();
		}
	}

	// **ForumRes**
	// 編集・削除のボタン処理
	protected void gvForumRes_RowCommand(object sender, GridViewCommandEventArgs e)
	{
		var ht = (Hashtable)Session["param"];

		// レス編集の投稿ボタン処理
		if (e.CommandName == "cResEditPost")
		{
			var source = (Control)e.CommandSource;
			var row = (GridViewRow)source.NamingContainer;

			var gvChild = (GridView)sender; // gvForumResDataをGridViewのChildとして呼び出す
			GridViewRow gvrParent = (GridViewRow)gvChild.NamingContainer; // gvForumDataをGridViewのParentとして

			// CommandArgumentとしてforum_res_idの取得
			var commandArg = (string)e.CommandArgument;
			string caNumericPart = commandArg.TrimStart('#');
			int forum_res_id = int.Parse(caNumericPart);

			TextBox tbResTitleEdit = (TextBox)row.FindControl("tbResTitleEdit");
			TextBox tbResBodyEdit = (TextBox)row.FindControl("tbResBodyEdit");
			Panel pnlResEdit = (Panel)row.FindControl("pnlResEdit");

			Panel pnlForumRes = (Panel)gvrParent.FindControl("pnlForumRes");

			if (tbResTitleEdit != null && tbResBodyEdit != null && pnlResEdit != null && pnlForumRes != null)
			{
				string str_forum_id = (string)gvForumData.DataKeys[gvrParent.RowIndex].Value;
				string fNumericPart = str_forum_id.TrimStart('#');
				int forum_id = int.Parse(fNumericPart);

				var result = ForumRes_Update((int)forum_res_id, (int)forum_id, (string)tbResTitleEdit.Text, (string)tbResBodyEdit.Text);
				if (result)
				{
					Label lResPoster = (Label)row.FindControl("lResPoster");
					if (lResPoster != null) lResPoster.Text = HtmlSanitizer.HtmlEncode((String)ht["name"]);
					RebindParentAndShowChild(forum_id);

					tbResTitleEdit.Text = "";
					tbResBodyEdit.Text = "";

					pnlResEdit.Visible = false;
				}
				else
				{
					lErrorMessage.Text = HtmlSanitizer.HtmlEncode("レス投稿の更新は失敗しました。");
				}
			}
		}
		// レス編集の表示ボタン処理
		else if (e.CommandName == "cResEdit")
		{
			var source = (Control)e.CommandSource;
			var row = (GridViewRow)source.NamingContainer;
			var gvChild = (GridView)sender; // gvForumResDataをGridViewのChildとして呼び出す

			Panel pnlResEdit = (Panel)row.FindControl("pnlResEdit");

			TextBox tbResEditTitle = (TextBox)row.FindControl("tbResTitleEdit");
			TextBox tbResEditBody = (TextBox)row.FindControl("tbResBodyEdit");

			if (pnlResEdit != null && tbResEditTitle != null && tbResEditBody != null)
			{
				if (pnlResEdit.Visible)
				{   // キャンセルと同様
					tbResEditTitle.Text = "";
					tbResEditBody.Text = "";
					pnlResEdit.Visible = false;
				}
				else
				{
					// 他の列は非表示
					foreach (GridViewRow r in gvChild.Rows)
					{
						Panel pnlResEdit_Other = (Panel)r.FindControl("pnlResEdit");
						if (pnlResEdit_Other != null) pnlResEdit_Other.Visible = false;
					}

					GridViewRow gvrParent = (GridViewRow)gvChild.NamingContainer; // gvForumDataをGridViewのParentとして
					string str_forum_id = (string)gvForumData.DataKeys[gvrParent.RowIndex].Value;
					string fNumericPart = str_forum_id.TrimStart('#');
					int forum_id = int.Parse(fNumericPart);

					string str_forum_res_id = (string)gvChild.DataKeys[row.RowIndex].Value;
					string frNumericPart = str_forum_res_id.TrimStart('#');
					int forum_res_id = int.Parse(frNumericPart);

					// 他の列は非表示
					foreach (GridViewRow r in gvForumData.Rows)
					{
						Panel pnlEdit_Other = (Panel)r.FindControl("pnlEdit");
						if (pnlEdit_Other != null) pnlEdit_Other.Visible = false;
					}

					var data = Get_ForumResByID((int)forum_id, (int)forum_res_id);

					tbResEditTitle.Attributes["placeholder"] = data["forum_res_title"].ToString();
					tbResEditBody.Attributes["placeholder"] = data["forum_res_text"].ToString();

					pnlResEdit.Visible = true;
				}
			}
		}
		// レス編集キャンセルボタン処理
		else if (e.CommandName == "cResEditCancel")
		{
			var source = (Control)e.CommandSource;
			var row = (GridViewRow)source.NamingContainer;

			Panel pnlResEdit = (Panel)row.FindControl("pnlResEdit");
			TextBox tbResTitleEdit = (TextBox)row.FindControl("tbResTitleEdit");
			TextBox tbResBodyEdit = (TextBox)row.FindControl("tbResBodyEdit");

			if (pnlResEdit != null && tbResTitleEdit != null && tbResBodyEdit != null)
			{
				tbResTitleEdit.Text = "";
				tbResBodyEdit.Text = "";
				pnlResEdit.Visible = false;
			}
		}
		// レス削除ボタン処理
		else if (e.CommandName == "cResDelete")
		{
			var source = (Control)e.CommandSource;
			var row = (GridViewRow)source.NamingContainer;
			var gvChild = (GridView)sender; // gvForumResDataをGridViewのChildとして呼び出す

			GridViewRow gvrParent = (GridViewRow)gvChild.NamingContainer; // gvForumDataをGridViewのParentとして
			string str_forum_id = (string)gvForumData.DataKeys[gvrParent.RowIndex].Value;
			string fNumericPart = str_forum_id.TrimStart('#');
			int forum_id = int.Parse(fNumericPart);

			string str_forum_res_id = (string)gvChild.DataKeys[row.RowIndex].Value;
			string frNumericPart = str_forum_res_id.TrimStart('#');
			int forum_res_id = int.Parse(frNumericPart);

			var result = ForumRes_Delete((int)forum_res_id, (int)forum_id);
			if (result)
			{
				Label lResPoster = (Label)row.FindControl("lResPoster");
				if (lResPoster != null) lResPoster.Text = HtmlSanitizer.HtmlEncode((String)ht["name"]);
				RebindParentAndShowChild(forum_id);
			}
		}
	}

	// レス編集後、その更新の表示
	private void RebindParentAndShowChild(int forum_id)
	{
		var ht = (Hashtable)Session["param"];
		// ページング状態の保持
		expanded_forum_id = forum_id;

		// 1) ParentのGridViewのBind
		Forum_BindData();

		// 2) DataKeyとしてforum_id呼び出し
		for (int i = 0; i < gvForumData.Rows.Count; i++)
		{
			string str_forum_id = (string)gvForumData.DataKeys[i].Value;
			string frNumericPart = str_forum_id.TrimStart('#');
			int this_forum_id = int.Parse(frNumericPart);
			if (this_forum_id == forum_id)
			{
				GridViewRow row = gvForumData.Rows[i];
				current_res_page = 0;
				Label lResPoster = (Label)row.FindControl("lResPoster");
				if (lResPoster != null) lResPoster.Text = HtmlSanitizer.HtmlEncode((String)ht["name"]);

				// 3) ChildのGridViewのBind
				ForumRes_BindData(row, forum_id, current_res_page, true);
				// ForumResの項目がNULLであればページングの非表示
				var data = Get_ForumRes_Count(this_forum_id);
				Repeater rptResPager = (Repeater)row.FindControl("rptResPager");
				Label lResPager = (Label)row.FindControl("lResPager");
				if ((int)data[0] == 0)
				{
					rptResPager.Visible = false;
					lResPager.Visible = false;
				}
				else
				{
					rptResPager.Visible = true;
					lResPager.Visible = true;
				}
				break;
			}
		}
	}

	// 列によるデータ処理
	// ログイン中の会員は編集・削除ができるかの判断
	protected void gvForum_RowDataBound(object sender, GridViewRowEventArgs e)
	{
		var ht = (Hashtable)Session["param"];

		if (e.Row.RowType == DataControlRowType.DataRow) 
		{
			DataRowView dv = (DataRowView)e.Row.DataItem;	// 現在の列を取得

			LinkButton lnkEdit = (LinkButton)e.Row.FindControl("lnkEdit");
			LinkButton lnkDelete = (LinkButton)e.Row.FindControl("lnkDelete");

			if (lnkEdit != null && lnkDelete != null)
			{
				if (ht == null) 
				{
					Response.Redirect("~/Default.aspx");
				}
				else if ((int)dv["user_id"] == (int)ht["user_id"])
				{
					lnkEdit.Visible = true;

					lnkDelete.Visible = true;
				}
				else if (ht["user_id"] == null)
				{
					var data = GetUser(ht["login_id"].ToString(), ht["password"].ToString());
					ht.Add("user_id", (int)data["user_id"]);
				}
			}
		}
	}

	// レス投稿の列処理
	// ログイン中の会員は編集・削除できるかの判断
	protected void gvForumRes_RowDataBound(object sender, GridViewRowEventArgs e)
	{
		if (e.Row.RowType != DataControlRowType.DataRow) return;

		var ht = (Hashtable)Session["param"];
		var dv = (DataRowView)e.Row.DataItem;

		// ParentのGridViewからforum_idの取得
		GridView gvChild = (GridView)sender;	// ForumRes	(ChildのGridView)
		GridViewRow parentRow = (GridViewRow)gvChild.NamingContainer;	// Forum (ParentのGridView)

		string str_forum_id = (string)gvForumData.DataKeys[parentRow.RowIndex].Value;
		string numericPart = str_forum_id.TrimStart('#');
		int forum_id = int.Parse(numericPart);

		LinkButton lnkResEdit = (LinkButton)e.Row.FindControl("lnkResEdit");
		LinkButton lnkResDelete = (LinkButton)e.Row.FindControl("lnkResDelete");

		if (lnkResEdit  != null && lnkResDelete != null)
		{
			// レス投稿IDと投稿者の取得
			var resCommandArg = (string)lnkResEdit.CommandArgument;
			// forum_res_idをParseしてから取得
			string rcaNumericPart = resCommandArg.TrimStart('#');
			int forum_res_id = int.Parse(rcaNumericPart);

			// forum_idとforum_res_idによるForumRes列の取得
			var forumres_data = Get_ForumResByID((int)forum_id, (int)forum_res_id);

			if (ht == null) 
			{
				Response.Redirect("~/Default.aspx");
			}
			else if ((int)forumres_data["user_id"] == (int)ht["user_id"])
			{
				lnkResEdit.Visible = true;

				lnkResDelete.Visible = true;
			}
		}
	}

	// ForumResページング処理
	protected void rptResPager_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		if (e.CommandName == "Page")
		{
			// CommandArgumentの“forumId｜インデックス”を読み込む
			string[] parts = Convert.ToString(e.CommandArgument).Split('|');
			if (parts.Length != 2) return;

			int forum_id, page_index;
			if (!Int32.TryParse(parts[0], out forum_id)) return;
			if (!Int32.TryParse(parts[1], out page_index)) return;

			Repeater rpt = (Repeater)source;
			GridViewRow row = (GridViewRow)rpt.NamingContainer;

			expanded_forum_id = forum_id;
			current_res_page = page_index;

			ForumRes_BindData(row, expanded_forum_id, current_res_page, true);
		}
	}

	// ページ番号のボタン処理
	protected void lnkPageNumber_Click(object sender, EventArgs e)
	{
		LinkButton lnkPageNumber = (LinkButton)sender;
		int new_page;

		if (!int.TryParse(lnkPageNumber.CommandArgument, out new_page))
		{
			int.TryParse(lnkPageNumber.Text.Trim(), out new_page);
		}

		var dvForumCount = Get_Forum_Count();
		int forumCount = Convert.ToInt32(dvForumCount[0]);
		int total_pages = (forumCount + PAGE_SIZE - 1) / PAGE_SIZE;
		if (total_pages < 1) total_pages = 1;

		current_page = Math.Max(1, Math.Min(new_page, total_pages));

		Forum_BindData();
	}

	// GridViewにForumデータの登録
	protected void Forum_BindData()
	{

		var dt = Get_Forum();

		gvForumData.DataSource = dt;
		gvForumData.DataBind();

		// Forumの項目がNULLであればページングの非表示
		var data = Get_Forum_Count();
		if ((int)data[0] == 0)
		{
			lPageNumber.Visible = false;
			pnlPageNumber.Visible = false;
		}
		else
		{
			lPageNumber.Visible = true;
			pnlPageNumber.Visible = true;
		}

		lnkPageNumber_Add();
	}

	protected void ForumRes_BindData(GridViewRow row, int forum_id, int res_page_id, bool make_visible)
	{
		Panel pnlRes = (Panel)row.FindControl("pnlRes");
		Panel pnlForumRes = (Panel)row.FindControl("pnlForumRes");
		GridView gvChild = (GridView)row.FindControl("gvForumResData");	// GridViewのChildとしてForumResの呼び出し
		if (pnlForumRes != null && gvChild != null)
		{
			current_res_page = res_page_id;
			gvChild.PageIndex = res_page_id;

			gvChild.DataSource = Get_ForumRes((int)forum_id);
			gvChild.DataBind();

			ForumRes_BindPager(row, forum_id);
			pnlForumRes.Visible = true;

			if (make_visible) pnlRes.Visible = true;
		}
	}

	// GridViewのChildのビルダー(ForumResページング処理)
	private void ForumRes_BindPager(GridViewRow row, int forum_id)
	{
		var forum_res_count = Get_ForumRes_Count((int)forum_id);
		int total_pages = (Convert.ToInt32(forum_res_count[0]) + RES_PAGE_SIZE - 1) / RES_PAGE_SIZE;
		if (total_pages < 1) total_pages = 1;

		// リピータのためのリスト
		var items = new List<object>();
		for (int page_index = 0; page_index < total_pages; page_index++)
		{
			items.Add(new
			{
				Text = (page_index + 1).ToString() + (page_index == current_res_page ? "" : " "),
				Command = forum_id.ToString() + "|" + page_index.ToString()
			});
		}

		Repeater rptResPager = (Repeater)row.FindControl("rptResPager");
		if (rptResPager != null)
		{
			rptResPager.DataSource = items;
			rptResPager.DataBind();

			// 現在のページリンクをオフにする
			int idx = current_res_page;
			if (idx >= 0 && idx < rptResPager.Items.Count)
			{
				var item = rptResPager.Items[idx];
				LinkButton lnkResPage = (LinkButton)item.FindControl("lnkResPage");
				if (lnkResPage != null) lnkResPage.Enabled = false;
			}
		}
	}

	// Forumのページ枚数による表示
	protected void lnkPageNumber_Add()
	{
		// Forumの数によるページ枚数の判断
		var dvForumCount = Get_Forum_Count();
		int forumCount = Convert.ToInt32(dvForumCount[0]);

		int number_of_pages = (forumCount + PAGE_SIZE - 1) / PAGE_SIZE;
		if (number_of_pages < 1) number_of_pages = 1;

		if (current_page >  number_of_pages) current_page = number_of_pages;
		if (current_page < 1) current_page = 1;

		// ページ枚数によるLinkButtonの追加
		pnlPageNumber.Controls.Clear();
		for (int i = 1; i <= number_of_pages; ++i)
		{
			var lnkPageNumber = new LinkButton
			{
				ID = "lnkPageNumber_" + i,
				Text = i.ToString() + " ",
				CausesValidation = false,
				CssClass = "pagination-button",
				CommandArgument = i.ToString()	// スペースを除く
			};
			lnkPageNumber.Click += lnkPageNumber_Click;

			if (i == current_page) lnkPageNumber.Enabled = false;
			pnlPageNumber.Controls.Add(lnkPageNumber);
		}
	}

	// タイトルの最大文字数
	protected void tbTitle_lengthValidate(object source, ServerValidateEventArgs args)
	{
		try
		{
			string i = args.Value;
			args.IsValid = (i.Length <= 15);
		}
		catch (Exception)
		{
			args.IsValid = false;
		}
	}

	// 本文の最小文字数と最大文字数
	protected void tbBody_lengthValidate(object source, ServerValidateEventArgs args)
	{
		try
		{
			string i = args.Value;
			args.IsValid = (i.Length <= 50);
		}
		catch (Exception)
		{
			args.IsValid = false;
		}
	}

	// レスタイトルの最大文字数
	protected void tbResTitle_lengthValidate(object source, ServerValidateEventArgs args)
	{
		try
		{
			string i = args.Value;
			args.IsValid = (i.Length <= 15);
		}
		catch (Exception)
		{
			args.IsValid = false;
		}
	}

	// レス本文の最小文字数と最大文字数
	protected void tbResBody_lengthValidate(object source, ServerValidateEventArgs args)
	{
		try
		{
			string i = args.Value;
			args.IsValid = (i.Length <= 50);
		}
		catch (Exception)
		{
			args.IsValid = false;
		}
	}

	// 編集タイトルの最大文字数
	protected void tbEditTitle_lengthValidate(object source, ServerValidateEventArgs args)
	{
		try
		{
			string i = args.Value;
			args.IsValid = (i.Length <= 15);
		}
		catch (Exception)
		{
			args.IsValid = false;
		}
	}

	// 編集本文の最小文字数と最大文字数
	protected void tbEditBody_lengthValidate(object source, ServerValidateEventArgs args)
	{
		try
		{
			string i = args.Value;
			args.IsValid = (i.Length <= 50);
		}
		catch (Exception)
		{
			args.IsValid = false;
		}
	}

	// 編集レスタイトルの最大文字数
	protected void tbResTitleEdit_lengthValidate(object source, ServerValidateEventArgs args)
	{
		try
		{
			string i = args.Value;
			args.IsValid = (i.Length <= 15);
		}
		catch (Exception)
		{
			args.IsValid = false;
		}
	}

	// 編集レス本文の最小文字数と最大文字数
	protected void tbResBodyEdit_lengthValidate(object source, ServerValidateEventArgs args)
	{
		try
		{
			string i = args.Value;
			args.IsValid = (i.Length <= 50);
		}
		catch (Exception)
		{
			args.IsValid = false;
		}
	}

	// ページリンクのEnabled処理
	private void lnkPageNumber_SetActive()
	{
		foreach (Control c in pnlPageNumber.Controls)
		{
			var lnk = c as LinkButton;
			if (lnk == null) continue;

			int page_num;
			// prefer CommandArgument; fall back to Text
			if (!Int32.TryParse(lnk.CommandArgument, out page_num))
				Int32.TryParse(lnk.Text.Trim(), out page_num);

			lnk.Enabled = (page_num != current_page);
		}
	}

	/// <summary>
	/// 投稿登録
	/// </summary>
	/// <param name="user_id">ユーザID(投稿者)</param>
	/// <param name="forum_title">タイトル</param>
	/// <param name="forum_text">本文</param>
	/// <returns>登録できた場合、掲示板の投稿</returns>
	private bool Forum_Insert(int user_id, string forum_title, string forum_text)
	{
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("Forum", "Insert"))
		{
			var ht = new Hashtable
			{
				{"user_id", user_id},
				{"forum_title", forum_title},
				{"forum_text", forum_text}
			};
			try
			{
				var result = statement.ExecStatementWithOC(accessor, ht);
				return (result > 0);
			}
			catch (Exception ex)
			{
				lErrorMessage.Text = HtmlSanitizer.HtmlEncode(ex.Message + "\n");
				return false;
			}
		}
	}

	/// <summary>
	/// 投稿編集
	/// </summary>
	/// <param name="forum_id">投稿ID</param>
	/// <param name="forum_title">タイトル</param>
	/// <param name="forum_text">本文</param>
	/// <returns>編集できた場合、投稿の更新</returns>
	private bool Forum_Update(int forum_id, string forum_title, string forum_text)
	{
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("Forum", "Update"))
		{
			var ht = new Hashtable
			{
				{"forum_id", forum_id},
				{"forum_title", forum_title},
				{"forum_text", forum_text}
			};
			try
			{
				var result = statement.ExecStatementWithOC(accessor, ht);
				return (result > 0);
			}
			catch (Exception ex)
			{
				lErrorMessage.Text = HtmlSanitizer.HtmlEncode(ex.Message + "\n");
				return false;
			}
		}
	}

	/// <summary>
	/// 投稿削除
	/// </summary>
	/// <param name="forum_id">投稿ID</param>
	/// <returns>読み込みできた場合、投稿の削除</returns>
	private bool Forum_Delete(int forum_id)
	{
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("Forum", "Delete"))
		{
			var ht = new Hashtable
			{
				{"forum_id", forum_id}
			};
			try
			{
				var result = statement.ExecStatementWithOC(accessor, ht);
				return (result > 0);
			}
			catch (Exception ex)
			{
				lErrorMessage.Text = HtmlSanitizer.HtmlEncode(ex.Message + "\n");
				return false;
			}
		}
	}

	/// <summary>
	/// 掲示板の投稿取得
	/// </summary>
	/// <param name="page_size">ページングのための枚数(10)</param>
	/// <param name="offset">補正</param>
	/// <returns>取得できた場合、掲示板のフォーマットされた列</returns>
	protected DataTable Get_Forum()
	{
		int offset = (current_page - 1) * PAGE_SIZE;

		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("Forum", "Get"))
		{
			var ht = new Hashtable
			{
				{"page_size", PAGE_SIZE},
				{"offset", offset}
			};
			var dt = statement.SelectStatementWithOC(accessor, ht);
			return (dt.Tables.Count != 0) ? dt.Tables[0] : null;
		}
	}

	protected DataRowView Get_ForumById(int forum_id)
	{
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("Forum", "Get_ById"))
		{
			var ht = new Hashtable
			{
				{"forum_id", forum_id}
			};
			var dv = statement.SelectSingleStatementWithOC(accessor, ht);
			return (dv.Count != 0) ? dv[0] : null;
		}
	}

	/// <summary>
	/// 投稿の個数取得
	/// </summary>
	/// <returns>取得できた場合、投稿の個数</returns>
	protected DataRowView Get_Forum_Count()
	{
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("Forum", "Get_Count"))
		{
			var dv = statement.SelectSingleStatementWithOC(accessor);
			return (dv.Count != 0) ? dv[0] : null;
		}
	}

	/// <summary>
	/// 投稿登録
	/// </summary>
	/// <param name="user_id">ユーザID(投稿者)</param>
	/// <param name="forum_res_title">タイトル</param>
	/// <param name="forum_res_text">本文</param>
	/// <returns>登録できた場合、掲示板のレス投稿</returns>
	protected bool ForumRes_Insert(int forum_id, int user_id, string forum_res_title, string forum_res_text)
	{
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("ForumRes", "Insert"))
		{
			var ht = new Hashtable
			{
				{"forum_id", forum_id},
				{"user_id", user_id},
				{"forum_res_title", forum_res_title},
				{"forum_res_text", forum_res_text}
			};

			var result = statement.ExecStatementWithOC(accessor, ht);
			return true;	// クエリは例外
		}
	}

	/// <summary>
	/// レス投稿の編集
	/// </summary>
	/// <param name="forum_res_id">レス投稿のID</param>
	/// <param name="forum_id">投稿のID</param>
	/// <param name="forum_res_title">編集レスタイトル</param>
	/// <param name="forum_res_text">編集レス本文</param>
	/// <returns>登録できた場合、レス投稿の更新</returns>
	protected bool ForumRes_Update(int forum_res_id, int forum_id, string forum_res_title, string forum_res_text)
	{
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("ForumRes", "Update"))
		{
			var ht = new Hashtable
			{
				{"forum_res_id", forum_res_id},
				{"forum_id", forum_id},
				{"forum_res_title", forum_res_title},
				{"forum_res_text", forum_res_text}
			};

			var result = statement.ExecStatementWithOC(accessor, ht);
			return (result > 0);
		}
	}

	/// <summary>
	/// レス投稿削除
	/// </summary>
	/// <param name="forum_res_id">レス投稿ID</param>
	/// <param name="forum_id">投稿ID</param>
	/// <returns>読み込みできた場合、レス投稿の削除</returns>
	private bool ForumRes_Delete(int forum_res_id, int forum_id)
	{
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("ForumRes", "Delete"))
		{
			var ht = new Hashtable
			{
				{"forum_res_id", forum_res_id},
				{"forum_id", forum_id}
			};
			try
			{
				var result = statement.ExecStatementWithOC(accessor, ht);
				return (result > 0);
			}
			catch (Exception ex)
			{
				lErrorMessage.Text = HtmlSanitizer.HtmlEncode(ex.Message + "\n");
				return false;
			}
		}
	}

	/// <summary>
	/// レス投稿取得
	/// </summary>
	/// <param name="page_size">ページングのための枚数(10)</param>
	/// <param name="offset">補正</param>
	/// <returns>取得できた場合、掲示板のフォーマットされた列</returns>
	protected DataTable Get_ForumRes(int forum_id)
	{
		int offset = (current_res_page) * RES_PAGE_SIZE;

		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("ForumRes", "Get"))
		{
			var ht = new Hashtable
			{
				{"forum_id", forum_id},
				{"offset", offset}
			};
			var dt = statement.SelectStatementWithOC(accessor, ht);
			return (dt.Tables.Count != 0) ? dt.Tables[0] : null;
		}
	}

	/// <summary>
	/// レス投稿の個数取得
	/// </summary>
	/// <returns>取得できた場合、レス投稿の個数</returns>
	protected DataRowView Get_ForumRes_Count(int forum_id)
	{
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("ForumRes", "Get_Count"))
		{
			var ht = new Hashtable { { "forum_id", forum_id } };
			var dv = statement.SelectSingleStatementWithOC(accessor, ht);
			return (dv.Count != 0) ? dv[0] : null;
		}
	}

	protected DataRowView Get_ForumResByID(int forum_id, int forum_res_id)
	{
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("ForumRes", "Get_ById"))
		{
			var ht = new Hashtable
			{
				{ "forum_id", forum_id },
				{ "forum_res_id", forum_res_id },
			};
			var dv = statement.SelectSingleStatementWithOC(accessor, ht);
			return (dv.Count != 0) ? dv[0] : null;
		}
	}

	/// <summary>
	/// ユーザ取得
	/// </summary>
	/// <param name="login_id">ログインID</param>
	/// <param name="password">パスワード</param>
	/// <returns>取得できた場合、会員情報</returns>
	protected DataRowView GetUser(string login_id, string password)
	{
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("User", "Get"))
		{
			var ht = new Hashtable
			{
				{"login_id", login_id},
				{"password", password}
			};
			var dv = statement.SelectSingleStatementWithOC(accessor, ht);
			return (dv.Count != 0) ? dv[0] : null;
		}
	}


}
