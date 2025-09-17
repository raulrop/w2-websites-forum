using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.Common.Sql;
using w2.Common.Web;

public partial class Form_User_UserRegisterConfirm : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		lErrorMessage.Text = "";

		var ht = (Hashtable)Session["param"];
		// 未ログインのエラー
		if (ht == null)
		{
			Response.Redirect("~/Default.aspx");
			return;
		}
		else if (ht["type"] != null)
		{
			// UserRegisterInputから遷移した場合
			if (ht["type"].ToString() == "Register")
			{
				lTitle.Text = "登録内容確認";
			}
			//　UserModifyInputから遷移した場合
			else if (ht["type"].ToString() == "Update")
			{
				lTitle.Text = "編集内容確認";
			}
		}
		lUserName.Text = HtmlSanitizer.HtmlEncode(ht["name"]);
		lLoginId.Text = HtmlSanitizer.HtmlEncode(ht["login_id"]);
		lPassword.Text = HtmlSanitizer.HtmlEncode(ht["password"]);
	}

	protected void btnUserRegisterConfirm_Click(object sender, EventArgs e)
	{
		var ht = (Hashtable)Session["param"];	// セッションのparamの存在は確認しなくていい(？)

		var name = ht["name"].ToString();
		var login_id = ht["login_id"].ToString();
		var password = ht["password"].ToString();
		// ユーザ登録情報の入力画面から遷移の場合
		if (ht["type"].ToString() == "Register")
		{
			var result = Insert(name, login_id, password);
			if (result)
			{
				ht.Remove("type");  // どこから遷移かの判断は不要
				var data = Get(ht["login_id"].ToString(), ht["password"].ToString());
				ht.Add("user_id", (int)data["user_id"]);
				Session["param"] = ht;
				Response.Redirect("~/Form/User/UserRegisterComplete.aspx");
			}
			else
			{
				lErrorMessage.Text += HtmlSanitizer.HtmlEncode("会員登録は失敗しました");
			}
		}
		else if ( ht["type"].ToString() == "Update")
		{
			var user_id = (int)ht["user_id"];	// ユーザID
			var result = Update(user_id, name, login_id, password);
			if (result)
			{
				ht.Remove("type");
				Session["param"] = ht;
				Response.Redirect("~/Form/User/UserRegisterComplete.aspx");
			}
			else
			{
				lErrorMessage.Text += HtmlSanitizer.HtmlEncode("会員情報の更新は失敗しました");
			}
		}
		// どこからの判断はない場合
		else
		{
			Response.Redirect("~/Form/Forum/ForumTop.aspx");
		}
	}

	protected void btnReturn_Click(object sender, EventArgs e)
	{
		Session.Clear();
		Response.Redirect("~/Form/User/UserRegisterInput.aspx");
	}

	/// <summary>
	/// ユーザー登録
	/// </summary>
	/// <param name="name">Name</param>
	/// <param name="login_id">Login ID</param>
	/// <param name="password">Password</param>
	/// <returns>登録できた場合、テーブルに会員登録情報の入力</returns>
	private bool Insert(string name, string login_id, string password)
	{
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("User", "Insert"))
		{
			var ht = new Hashtable
			{
				{"name", name},
				{"login_id", login_id},
				{"password", password}
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
	/// ユーザ情報の更新
	/// </summary>
	/// <param name="name">Name</param>
	/// <param name="login_id">Login ID</param>
	/// <param name="password">Password</param>
	/// <returns>登録できた場合、テーブルに登録済みの会員情報の更新</returns>
	private bool Update(int user_id, string name, string login_id, string password)
	{
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("User", "Update"))
		{
			var ht = new Hashtable
			{
				{"user_id", user_id},
				{"name", name},
				{"login_id", login_id},
				{"password", password}
			};
			var result = statement.ExecStatementWithOC(accessor, ht);
			return (result > 0);
		}
	}

	/// <summary>
	/// ユーザ取得
	/// </summary>
	/// <param name="login_id">ログインID</param>
	/// <param name="password">パスワード</param>
	/// <returns>取得できた場合、会員情報</returns>
	protected DataRowView Get(string login_id, string password)
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