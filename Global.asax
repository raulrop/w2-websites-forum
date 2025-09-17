<%@ Application Language="C#" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Web.Configuration" %>
<%@ Import Namespace="w2.Common" %>
<%@ Import Namespace="w2.Common.Logger" %>

<script runat="server">

	/// <summary>
	/// アプリケーションのスタートアップで実行するコードです
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
    void Application_Start(object sender, EventArgs e)
	{
		Initialize();
	}

	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		Constants.APPLICATION_NAME = WebConfigurationManager.AppSettings["Application_Name"];
		Constants.STRING_SQL_CONNECTION = WebConfigurationManager.AppSettings["ConnectionString"];
		Constants.PHYSICALDIRPATH_LOGFILE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
		Constants.PHYSICALDIRPATH_SQL_STATEMENT = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Xml", "Db");
	}
    
    void Application_End(object sender, EventArgs e) 
    {
    }

	/// <summary>
	/// ハンドルされていないエラーが発生したときに実行するコード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Application_Error(object sender, EventArgs e)
	{
		// 最後に発生したエラー原因情報をExceptionオブジェクトとして取得
		var error = Server.GetLastError();

		// ログ出力
		var errorPageInfo = w2.Common.Web.WebUtility.GetRawUrl(Request) + "   [" + Request.UserHostAddress + "] [" + w2.Common.Util.StringUtility.ToEmpty(Request.UserAgent) + "]";
		FileLogger.WriteError(errorPageInfo, error);
	}

	void Session_Start(object sender, EventArgs e) 
    {
        // 新規セッションを開始したときに実行するコードです
    }

    void Session_End(object sender, EventArgs e) 
    {
        // セッションが終了したときに実行するコードです。
        // メモ: Session_End イベントは、Web.config ファイル内で sessionstate モードが
        // InProc に設定されているときにのみ発生します。session モードが StateServer か、または 
        // SQLServer に設定されている場合、このイベントは発生しません。

    }
       
</script>
