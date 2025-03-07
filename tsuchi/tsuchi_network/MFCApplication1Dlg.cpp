
// MFCApplication1Dlg.cpp : 実装ファイル
//

#include "pch.h"
#include "framework.h"
#include "MFCApplication1.h"
#include "MFCApplication1Dlg.h"
#include "afxdialogex.h"
#include <winsock2.h>
#include <ws2tcpip.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

#define WM_MSG_COMMSTS	(WM_APP + 1)

bool g_commTerminate = false;
wchar_t* g_pRecvBuff = NULL;
char* g_pSendBuff = NULL;
size_t g_sendSize = 0;

DWORD WINAPI ThreadFunc(LPVOID arg);

// アプリケーションのバージョン情報に使われる CAboutDlg ダイアログ

class CAboutDlg : public CDialogEx
{
public:
	CAboutDlg();

// ダイアログ データ
#ifdef AFX_DESIGN_TIME
	enum { IDD = IDD_ABOUTBOX };
#endif

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV サポート

// 実装
protected:
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialogEx(IDD_ABOUTBOX)
{
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialogEx)
END_MESSAGE_MAP()


// CMFCApplication1Dlg ダイアログ



CMFCApplication1Dlg::CMFCApplication1Dlg(CWnd* pParent /*=nullptr*/)
	: CDialogEx(IDD_MFCAPPLICATION1_DIALOG, pParent)
{
	m_commStart = false;
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CMFCApplication1Dlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CMFCApplication1Dlg, CDialogEx)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDOK, &CMFCApplication1Dlg::OnBnClickedOk)
	ON_BN_CLICKED(IDCOMMU, &CMFCApplication1Dlg::OnBnClickedCommu)
	ON_NOTIFY(IPN_FIELDCHANGED, IDC_IPADDRESS1, &CMFCApplication1Dlg::OnIpnFieldchangedIpaddress1)
	ON_BN_CLICKED(IDCOMMU, &CMFCApplication1Dlg::OnBnClickedCommu)
	ON_BN_CLICKED(IDCANCEL, &CMFCApplication1Dlg::OnBnClickedCancel)
	ON_BN_CLICKED(IDC_BT_SEND, &CMFCApplication1Dlg::OnBnClickedSend)
	// user message
	ON_MESSAGE(WM_MSG_COMMSTS, &CMFCApplication1Dlg::OnCommEnd)
END_MESSAGE_MAP()


// CMFCApplication1Dlg メッセージ ハンドラー

BOOL CMFCApplication1Dlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// "バージョン情報..." メニューをシステム メニューに追加します。

	// IDM_ABOUTBOX は、システム コマンドの範囲内になければなりません。
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != nullptr)
	{
		BOOL bNameValid;
		CString strAboutMenu;
		bNameValid = strAboutMenu.LoadString(IDS_ABOUTBOX);
		ASSERT(bNameValid);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	// このダイアログのアイコンを設定します。アプリケーションのメイン ウィンドウがダイアログでない場合、
	//  Framework は、この設定を自動的に行います。
	SetIcon(m_hIcon, TRUE);			// 大きいアイコンの設定
	SetIcon(m_hIcon, FALSE);		// 小さいアイコンの設定

	// TODO: 初期化をここに追加します。
	LoadIniFile();

	return TRUE;  // フォーカスをコントロールに設定した場合を除き、TRUE を返します。
}

void CMFCApplication1Dlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CDialogEx::OnSysCommand(nID, lParam);
	}
}

// ダイアログに最小化ボタンを追加する場合、アイコンを描画するための
//  下のコードが必要です。ドキュメント/ビュー モデルを使う MFC アプリケーションの場合、
//  これは、Framework によって自動的に設定されます。

void CMFCApplication1Dlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // 描画のデバイス コンテキスト

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// クライアントの四角形領域内の中央
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// アイコンの描画
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialogEx::OnPaint();
	}
}

// ユーザーが最小化したウィンドウをドラッグしているときに表示するカーソルを取得するために、
//  システムがこの関数を呼び出します。
HCURSOR CMFCApplication1Dlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}


void CMFCApplication1Dlg::OnBnClickedOk()
{
	// TODO: ここにコントロール通知ハンドラー コードを追加します。
	MessageBox(L"Cleck OK Button", L"SUCCESS", MB_OK | MB_ICONINFORMATION);
	CDialogEx::OnOK();

}


void CMFCApplication1Dlg::OnBnClickedCommu()
{
	// TODO: ここにコントロール通知ハンドラー コードを追加します。
	if (!m_commStart)
	{
		GetDlgItem(IDC_ED_RECV)->SetWindowTextW(L"");
		m_recvTotalSize = 0;
		if (!CheckInputInformation())
			return;

		GetDlgItem(IDC_ST_STATUS)->SetWindowTextW(L"Start of communication");
		GetDlgItem(IDCOMMU)->EnableWindow(false);
		GetDlgItem(IDCANCEL)->EnableWindow(false);
		Sleep(1000);
		if (TcpServerStart()) {
			GetDlgItem(IDCOMMU)->SetWindowTextW(L"STOP");
			GetDlgItem(IDCOMMU)->EnableWindow(true);
			m_commStart = true;
			g_commTerminate = false;
		}
		else
		{
			GetDlgItem(IDC_ST_STATUS)->SetWindowTextW(L"End of communication");
			GetDlgItem(IDCOMMU)->EnableWindow(true);
			GetDlgItem(IDCANCEL)->EnableWindow(true);
		}
	}
	else
	{
		g_commTerminate = true;
	}
}

void CMFCApplication1Dlg::OnBnClickedCancel()
{
	// TODO: ここにコントロール通知ハンドラー コードを追加します。
	UpdateIniFile();

	CDialogEx::OnCancel();
}

void CMFCApplication1Dlg::OnBnClickedSend()
{
	CString sendMsg;
	GetDlgItem(IDC_ED_SEND)->GetWindowText(sendMsg);
	if (sendMsg == L"")
		return;

	sendMsg += "\n";
	int bufSize = GetDlgItem(IDC_ED_SEND)->GetWindowTextLength() * 2;
	g_pSendBuff = new char[bufSize + 1];
	if (wcstombs_s(&g_sendSize, g_pSendBuff, bufSize, sendMsg, _TRUNCATE))
	{
		delete g_pSendBuff;
		g_pSendBuff = NULL;
		g_sendSize = 0;
		MessageBox(L"Send buffer allocation error.", L"ERROR", MB_OK | MB_ICONERROR);
		return;
	}

	GetDlgItem(IDC_ED_SEND)->EnableWindow(false);
	GetDlgItem(IDC_BT_SEND)->EnableWindow(false);
}

void CMFCApplication1Dlg::OnIpnFieldchangedIpaddress1(NMHDR* pNMHDR, LRESULT* pResult)
{
	LPNMIPADDRESS pIPAddr = reinterpret_cast<LPNMIPADDRESS>(pNMHDR);
	// TODO: ここにコントロール通知ハンドラー コードを追加します。
	*pResult = 0;
}

void CMFCApplication1Dlg::LoadIniFile()
{
	GetModuleFileName(NULL, m_exeFilePath, MAX_PATH);
	TCHAR* ptmp = _tcsrchr(m_exeFilePath, _T('\\')); // \の最後の出現位置を取得
	if (ptmp != NULL)
	{   //ファイル名を削除
		ptmp = _tcsinc(ptmp);   //一文字進める
		*ptmp = _T('\0');
	}
	StrCatBuff(m_exeFilePath, INI_FILE_NAME, sizeof(m_exeFilePath)/2);

	if (PathFileExists(m_exeFilePath)) {
		// load ip address
		WCHAR strBuf[16] = { 0 };
		WCHAR* pStr = strBuf;
		BYTE ipset[4] = { 0 };
		if (::GetPrivateProfileString(L"CONECTINFO", L"IP", NULL, strBuf, _countof(strBuf), m_exeFilePath)) {
			for (int i = 0; i < 4; i++)
			{
				CString strAddr = pStr;
				int findPos = strAddr.Find(L".", 0);
				if (findPos <= 0)
				{
					ipset[i] = _ttoi(pStr);
					break;
				}

				pStr[findPos++] = 0;
				ipset[i] = _ttoi(pStr);
				pStr += findPos;
			}
			CIPAddressCtrl* ipCtrl = (CIPAddressCtrl*)GetDlgItem(IDC_IPADDRESS1);
			ipCtrl->SetAddress(ipset[0], ipset[1], ipset[2], ipset[3]);
		}

		// load port number
		if (::GetPrivateProfileString(L"CONECTINFO", L"PORT", NULL, strBuf, _countof(strBuf), m_exeFilePath)) {
			GetDlgItem(IDC_ED_PORT)->SetWindowTextW(strBuf);
		}
	}
}
void CMFCApplication1Dlg::UpdateIniFile()
{
	BYTE ipset[4];
	CIPAddressCtrl* ipCtrl = (CIPAddressCtrl*)GetDlgItem(IDC_IPADDRESS1);
	ipCtrl->GetAddress(ipset[0], ipset[1], ipset[2], ipset[3]);
	m_ipAddress.Format(L"%d.%d.%d.%d", ipset[0], ipset[1], ipset[2], ipset[3]);
	if(!::WritePrivateProfileString(L"CONECTINFO", L"IP", m_ipAddress, m_exeFilePath))
		return;

	CString portNumber;
	GetDlgItem(IDC_ED_PORT)->GetWindowTextW(portNumber);
	::WritePrivateProfileString(L"CONECTINFO", L"PORT", portNumber, m_exeFilePath);
}

LRESULT CMFCApplication1Dlg::OnCommEnd(WPARAM wParam, LPARAM lParam)
{
	CString msg;
	CString strTmp;
	CEdit* pEdit = (CEdit*)GetDlgItem(IDC_ED_RECV);
	int nLastPos;

	switch (wParam)
	{
	case COMM_ACCEPT:
		GetDlgItem(IDC_ST_STATUS)->SetWindowTextW(L"Connection accepted.");
		GetDlgItem(IDC_BT_SEND)->EnableWindow(true);
		return 0;

	case COMM_RECVING:
		m_recvTotalSize += (DWORD)lParam;
		msg.Format(L"Receiving... %lubyte(total %luByte)", (DWORD)lParam, m_recvTotalSize);

		GetDlgItem(IDC_ST_STATUS)->SetWindowTextW(msg);
		nLastPos = pEdit->GetWindowTextLength();
		pEdit->SetSel(nLastPos, nLastPos);
		pEdit->ReplaceSel((LPCTSTR)g_pRecvBuff);
		delete g_pRecvBuff;
		g_pRecvBuff = NULL;
		return 0;

	case COMM_SEND:
		GetDlgItem(IDC_ED_SEND)->GetWindowText(msg);
		strTmp.Format(L"          %s\r\n", msg);

		nLastPos = pEdit->GetWindowTextLength();
		pEdit->SetSel(nLastPos, nLastPos);
		pEdit->ReplaceSel(strTmp);

		GetDlgItem(IDC_ED_SEND)->SetWindowText(L"");
		GetDlgItem(IDC_ED_SEND)->EnableWindow(true);
		GetDlgItem(IDC_BT_SEND)->EnableWindow(true);
		return 0;

	case COMM_DISCONNECT:
		msg = L"Disconnected from client.";
		break;
	case COMM_TERMINATE:
		msg = L"Communication terminate.";
		break;

	case COMM_ERR_SELECT_A:
		msg = L"Error: select accept";
		break;
	case COMM_ERR_TIMEOUT_A:
		msg = L"Error: select timeout";
		break;
	case COMM_ERR_ACCEPT:
		msg = L"Error: accept";
		break;
	case COMM_ERR_SELECT_R:
		msg = L"Error: select recv";
		break;
	case COMM_ERR_TIMEOUT_R:
		msg = L"Error: recv timeout";
		break;
	case COMM_ERR_RECV:
		msg = L"Error: recv";
		break;
	case COMM_ERR_SEND:
		GetDlgItem(IDC_ED_SEND)->EnableWindow(true);
		GetDlgItem(IDC_BT_SEND)->EnableWindow(true);
		msg = L"Error: send";
		break;
	}

	CloseSocket();

	if (wParam < 0)
		MessageBox(msg, L"ERROR", MB_OK | MB_ICONERROR);
	else
		MessageBox(msg, L"FINISH", MB_OK | MB_ICONINFORMATION);

	WaitForSingleObject(m_hThread, INFINITE);
	CloseHandle(m_hThread);
	m_commStart = false;
	g_commTerminate = false;

	GetDlgItem(IDC_ST_STATUS)->SetWindowTextW(L"End of communication");
	GetDlgItem(IDCOMMU)->SetWindowTextW(L"START");
	GetDlgItem(IDCOMMU)->EnableWindow(true);
	GetDlgItem(IDCANCEL)->EnableWindow(true);
	GetDlgItem(IDC_BT_SEND)->EnableWindow(false);

	return 0;
}

BOOL CMFCApplication1Dlg::CheckInputInformation()
{
	BYTE ipset[4];
	CIPAddressCtrl* ipCtrl = (CIPAddressCtrl*)GetDlgItem(IDC_IPADDRESS1);
	ipCtrl->GetAddress(ipset[0], ipset[1], ipset[2], ipset[3]);

	if (!ipset[0] && !ipset[1] && !ipset[2] && !ipset[3])
	{
		MessageBox(L"The IP address is missing.", L"INPUT ERROR", MB_OK | MB_ICONERROR);
		return false;
	}
	m_ipAddress.Format(L"%d.%d.%d.%d", ipset[0], ipset[1], ipset[2], ipset[3]);

	CString portNumber;
	GetDlgItem(IDC_ED_PORT)->GetWindowTextW(portNumber);
	m_portNumber = _ttoi(portNumber);
	if (!m_portNumber)
	{
		MessageBox(L"The port number is missing.", L"INPUT ERROR", MB_OK | MB_ICONERROR);
		return false;
	}

	return true;
}

bool CMFCApplication1Dlg::TcpServerStart()
{
	WSADATA wsaData;
	if (NO_ERROR != WSAStartup(MAKEWORD(2, 2), &wsaData))
	{
		MessageBox(L"Error: WSAStartup", L"ERROR", MB_OK | MB_ICONERROR);
		return false; //異常終了
	}

	// create socket
	m_socket = socket(AF_INET, SOCK_STREAM, 0); //アドレスドメイン, ソケットタイプ, プロトコル
	if (m_socket == INVALID_SOCKET) { //エラー処理
		MessageBox(L"Error: socket cleate", L"ERROR", MB_OK | MB_ICONERROR);
		CloseSocket();
		return false; //異常終了
	}

	// create socket address
	sockaddr_in addr;
	memset(&addr, 0, sizeof(struct sockaddr_in));
	addr.sin_family = AF_INET;
	addr.sin_port = htons((WORD)m_portNumber);
	InetPton(addr.sin_family, m_ipAddress, &addr.sin_addr.S_un.S_addr);

	// Bind to the port
	if (bind(m_socket, (struct sockaddr*)&addr, sizeof(addr)) < 0) {

		MessageBox(L"Error: bind", L"ERROR", MB_OK | MB_ICONERROR);
		CloseSocket();
		return false; //異常終了
	}

	// Start accepting connections
	if (listen(m_socket, SOMAXCONN) < 0) { //ソケット, キューの最大長 //エラー処理

		MessageBox(L"Error: listen", L"ERROR", MB_OK | MB_ICONERROR);
		CloseSocket();
		return false; //異常終了
	}

	// Start a thread to communicate with the client
	m_hThread = CreateThread(
		NULL,			//セキュリティ属性
		0,				//スタックサイズ
		ThreadFunc,		//スレッド関数
		this,		//スレッド関数に渡す引数
		0,				//作成オプション(0またはCREATE_SUSPENDED)
		&m_dwThreadId);	//スレッドID
	if (m_hThread == NULL) {
		MessageBox(L"Error: CreateThread", L"ERROR", MB_OK | MB_ICONERROR);
		CloseSocket();
		return false; //異常終了
	}
	
	GetDlgItem(IDC_ST_STATUS)->SetWindowTextW(L"Waiting for connection...");
	return true;
}
void CMFCApplication1Dlg::CloseSocket()
{
	if (m_socket != INVALID_SOCKET)
		closesocket(m_socket);
	
	WSACleanup();
	m_socket = INVALID_SOCKET;
}

DWORD WINAPI ThreadFunc(LPVOID arg)
{
	CMFCApplication1Dlg* dlg = (CMFCApplication1Dlg*)arg;
	SOCKET sockfd = dlg->GetCommSocket();

	timeval tv;
	fd_set fds, readfds;
	int ret_select;
	int timeCnt = 0;

	/* 読み込みFD集合を空にする */
	FD_ZERO(&readfds);
	/* 読み込みFD集合にsockfdを追加 */
	FD_SET(sockfd, &readfds);
	/* タイムアウト時間を設定 */
	tv.tv_sec = 1;
	tv.tv_usec = 0;

	while (!g_commTerminate)
	{

		fds = readfds;
		ret_select = select(1, &fds, NULL, NULL, &tv);
		if (ret_select == -1) {
			/* select関数がエラー */
			PostMessage(dlg->m_hWnd, WM_MSG_COMMSTS, (WPARAM)COMM_ERR_SELECT_A, 0);
			return 0; //異常終了
		}
		if (ret_select == 0) {
			/* FDの数が0なのでタイムアウトと判断 */
			if (++timeCnt < 60)
				continue;

			/* Timeout after 60 seconds(1sec x 60) */
			PostMessage(dlg->m_hWnd, WM_MSG_COMMSTS, (WPARAM)COMM_ERR_TIMEOUT_A, 0);
			return 0; //異常終了
		}
		break;
	}
	if (g_commTerminate) {
		PostMessage(dlg->m_hWnd, WM_MSG_COMMSTS, (WPARAM)COMM_TERMINATE, 0);
		return 0;
	}

	//接続待ち
	struct sockaddr_in get_addr; //接続相手のソケットアドレス
	int len = sizeof(struct sockaddr_in); //接続相手のアドレスサイズ
	SOCKET connect = accept(sockfd, (struct sockaddr*)&get_addr, &len); //接続待ちソケット, 接続相手のソケットアドレスポインタ, 接続相手のアドレスサイズ

	if (connect < 0) { //エラー処理
		PostMessage(dlg->m_hWnd, WM_MSG_COMMSTS, (WPARAM)COMM_ERR_ACCEPT, 0);
		return 0; //異常終了
	}
	PostMessage(dlg->m_hWnd, WM_MSG_COMMSTS, (WPARAM)COMM_ACCEPT, 0);

	//受信
	char str[1024] = { 0 }; //受信用データ格納用
	int rcvsize = 0;
	int totalSize = 0;
	timeCnt = 0;
	FD_ZERO(&readfds);
	FD_SET(connect, &readfds);

	while (!g_commTerminate)
	{
		// Check send data
		if (g_pSendBuff)
		{
			if (0 >= send(connect, g_pSendBuff, (int)g_sendSize, 0))
				PostMessage(dlg->m_hWnd, WM_MSG_COMMSTS, (WPARAM)COMM_ERR_SEND, 0);
			else
				PostMessage(dlg->m_hWnd, WM_MSG_COMMSTS, (WPARAM)COMM_SEND, 0);

			delete g_pSendBuff;
			g_pSendBuff = NULL;
			g_sendSize = 0;
		}

		/* タイムアウト時間を設定 */
		fds = readfds;
		tv.tv_sec = 1;
		tv.tv_usec = 0;
		int ret_select = select(1, &fds, NULL, NULL, &tv);

		/* 戻り値をチェック */
		if (ret_select == -1) {
			/* select関数がエラー */
			closesocket(connect);
			PostMessage(dlg->m_hWnd, WM_MSG_COMMSTS, (WPARAM)COMM_ERR_SELECT_R, 0);
			return 0;
		}
		if (ret_select == 0) {
			/* 読み込み可能になったFDの数が0なのでタイムアウトと判断 */
			if (++timeCnt < 60)
				continue;

			/* Timeout after 30 seconds(1sec x 60) */
			closesocket(connect);
			PostMessage(dlg->m_hWnd, WM_MSG_COMMSTS, (WPARAM)COMM_ERR_TIMEOUT_R, 0);
			return 0;
		}

		timeCnt = 0;
		rcvsize = recv(connect, str, sizeof(str), 0); //受信
		if (rcvsize <= 0)
		{
			break;
		}

		while (g_pRecvBuff)
		{
			Sleep(100);
		}
		g_pRecvBuff = new wchar_t[1024];
		memset(g_pRecvBuff, 0, 1024);
		MultiByteToWideChar(CP_ACP, 0, str, -1, g_pRecvBuff, sizeof(str)/2);
		PostMessage(dlg->m_hWnd, WM_MSG_COMMSTS, (WPARAM)COMM_RECVING, (LPARAM)rcvsize);
	}

	if (g_commTerminate) {
		PostMessage(dlg->m_hWnd, WM_MSG_COMMSTS, (WPARAM)COMM_TERMINATE, 0);
	}
	else if (rcvsize < 0) {
		PostMessage(dlg->m_hWnd, WM_MSG_COMMSTS, (WPARAM)COMM_ERR_RECV, 0);
	}
	else {
		PostMessage(dlg->m_hWnd, WM_MSG_COMMSTS, (WPARAM)COMM_DISCONNECT, 0);
	}

	//ソケットクローズ
	closesocket(connect);
	return 0;
}

