
// MFCApplication1Dlg.h : ヘッダー ファイル
//

#pragma once

#define INI_FILE_NAME L"tcpsrvapl.ini"
enum {
	COMM_ACCEPT = 0,
	COMM_RECVING,
	COMM_SEND,
	COMM_DISCONNECT,
	COMM_TERMINATE,

	COMM_ERR_SELECT_A	= -1,
	COMM_ERR_TIMEOUT_A	= -2,
	COMM_ERR_ACCEPT		= -3,
	COMM_ERR_SELECT_R	= -4,
	COMM_ERR_TIMEOUT_R	= -5,
	COMM_ERR_RECV		= -6,
	COMM_ERR_SEND		= -7
};

// CMFCApplication1Dlg ダイアログ
class CMFCApplication1Dlg : public CDialogEx
{
// コンストラクション
public:
	CMFCApplication1Dlg(CWnd* pParent = nullptr);	// 標準コンストラクター

// ダイアログ データ
#ifdef AFX_DESIGN_TIME
	enum { IDD = IDD_MFCAPPLICATION1_DIALOG };
#endif

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV サポート


// 実装
protected:
	HICON m_hIcon;

	// 生成された、メッセージ割り当て関数
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()

	void LoadIniFile();
	void UpdateIniFile();
	bool TcpServerStart();
	BOOL CheckInputInformation();

	CString m_ipAddress;
	DWORD m_portNumber;

	SOCKET m_socket;
	HANDLE m_hThread;
	DWORD m_dwThreadId;

	bool m_commStart;
	DWORD m_recvTotalSize;

	wchar_t m_exeFilePath[MAX_PATH];

public:
	afx_msg void OnBnClickedOk();
	afx_msg void OnBnClickedCommu();
	afx_msg void OnIpnFieldchangedIpaddress1(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnBnClickedCancel();
	afx_msg void OnBnClickedSend();

	afx_msg LRESULT OnCommEnd(WPARAM wParam, LPARAM lParam);

	SOCKET GetCommSocket() { return m_socket; }
	void CloseSocket();
};
