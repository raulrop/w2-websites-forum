using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.Common.Sql;
using w2.Common.Web;

public partial class Form_User_UserWithdrawalInput : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		lErrorMessage.Text = "";

		var ht = (Hashtable)Session["param"];
		// 未ログインのエラー表示
		if (ht == null)
		{
			Response.Redirect("~/Default.aspx");
		}
	}

	protected void btnDelete_Click(object sender, EventArgs e)
	{
		var ht = (Hashtable)Session["param"];

		var user_id = (int)ht["user_id"];

		var result = Delete(user_id);
		if (result)
		{
			Response.Redirect("~/Form/User/UserWithdrawalComplete.aspx");
		}
		else
		{
			lErrorMessage.Text += HtmlSanitizer.HtmlEncode("会員情報の削除は失敗しました");
		}
	}
	protected void btnCancel_Click(object sender, EventArgs e)
	{
		Response.Redirect("~/Form/Forum/ForumTop.aspx");
	}

	/// <summary>
	/// ユーザ情報の削除
	/// </summary>
	/// <param name="name">Name</param>
	/// <param name="login_id">Login ID</param>
	/// <param name="password">Password</param>
	/// <returns>会員のdelete_flgを「１(削除済み)」</returns>
	private bool Delete(int user_id)
	{
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("User", "Delete"))
		{
			var ht = new Hashtable
			{ 
				{"user_id", user_id}
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
}