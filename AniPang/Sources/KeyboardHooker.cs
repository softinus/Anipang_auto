/*	
 * Tobin Titus <tobin@daxsoftware.com>에 의해 작성된 코드를 수정함.
 * 오후 10:33 2004-12-14, Kenial_at_shinbiro_com 
 * 
 * 원 소스 출처 :
 *		GotDotNet User Sample: Low Level Keyboard Capture 
 *		http://www.gotdotnet.com/Community/UserSamples/Details.aspx?SampleGuid=55e8d5b3-0e53-47eb-99a9-ee65ed45f251
 * 
 */

using System;
using System.Runtime.InteropServices;
using System.Reflection;

namespace System.Hooks
{
	public class KeyboardHooker
	{
		// 후킹된 키보드 이벤트를 처리할 이벤트 핸들러
		private delegate long HookedKeyboardEventHandler(	int nCode, 	int wParam, IntPtr lParam );

		// 유저에게 노출할 이벤트 핸들러
		// bIsKeyDown : 현재 입력이 KeyDown/KeyUp인지 여부
		// bAlt, bCtrl, bShift, bWindowKey : 키 마스크
		// vkCode : virtual key 값, System.Windows.Forms.Key의 상수 값을
		// int로 변환해서 대응시키면 된다.
		public delegate long HookedKeyboardUserEventHandler(
			bool bIsKeyDown, bool bAlt, bool bCtrl, bool bShift, bool bWindowKey, int vkCode );
		//	후킹된 모듈의 핸들. 후킹이 성공했는지 여부를 식별하기 위해서 사용
		private static long								m_hDllKbdHook;
		private static bool								m_Hooked			= false;
		private static KBDLLHOOKSTRUCT					m_KbDllHs			= new KBDLLHOOKSTRUCT();
		private static IntPtr							m_LastWindowHWnd;
		private static IntPtr							m_CurrentWindowHWnd;
		
		// 후킹한 메시지를 받을 이벤트 핸들러
		private static HookedKeyboardEventHandler		m_LlKbEh
			= new HookedKeyboardEventHandler(HookedKeyboardProc);
		
		// 콜백해줄 이벤트 핸들러 ; 사용자측에 이벤트를 넘겨주기 위해서 사용
		private static HookedKeyboardUserEventHandler		m_fpCallbkProc = null;

		// 상수 선언
		private const int WH_KEYBOARD_LL	= 13;			// Intalls a hook procedure that monitors low-level keyboard input events.
		private const int HC_ACTION			= 0;			// Valid return for nCode parameter of LowLevelKeyboardProc

		private const int VK_TAB			= 0x09;			// Used to check the state of the Tab key
		private const int VK_CONTROL		= 0x11;			// Used to check the state of the Control key
		private const int VK_ESCAPE			= 0x1b;			// Used to check the state of the Escape key
		private const int VK_SHIFT			= 0x10;			// Used to check the state of the Shift key
		private const int VK_MENU			= 0x12;			// alt 
		private const int VK_LWIN			= 0x5B;			// window key		
		private const int VK_RWIN			= 0x5C;

		private const uint KEYSTATE_DOWN	= 0xffff8001;	// State of a key while pressed
		private const uint KEYSTATE_UP		= 0x0;			// State of a key when not pressed
		
		// 후킹되었는지의 여부
		public static bool Hooked
		{
				get	{	return m_Hooked;		}
			set	{	m_Hooked = value;		}
		}
		
		private static long HookedKeyboardProc(	int nCode, int wParam, IntPtr lParam )
		{
			long lResult = 0;
			// nCode should always be 0
			if(nCode == HC_ACTION)
			{
				// need to go to unsafe code to get sizeof(kbdllhookstruct)
				unsafe
				{
					CopyMemory(ref m_KbDllHs, lParam, sizeof(KBDLLHOOKSTRUCT));
				}

				//
				// set our current window handle to the foreground window
				// this is a global handle. Using GetActiveWindow only returns
				// windows in the current process.
				m_CurrentWindowHWnd = GetForegroundWindow();

				// If this isn't the same window as the last one, log new window information
				if( m_CurrentWindowHWnd != m_LastWindowHWnd )
				{
					m_LastWindowHWnd = m_CurrentWindowHWnd;
				}

				// 이벤트 발생
				if(m_fpCallbkProc!=null)
				{
					bool bAlt = (GetAsyncKeyState(VK_MENU) > 0);
					bool bCtrl = (GetAsyncKeyState(VK_CONTROL) > 0);
					bool bShift = (GetAsyncKeyState(VK_SHIFT) > 0);
					bool bWindowKey = (GetAsyncKeyState(VK_LWIN) > 0);
					
					lResult = m_fpCallbkProc(
						(m_KbDllHs.flags == 0),
						bAlt, bCtrl, bShift, bWindowKey,
						m_KbDllHs.vkCode);
				}

			}
			else if(nCode < 0)
			{
				//
				// MSDN Documentation:
				#region MSDN Documentation on return conditions
				// "If nCode is less than zero, the hook procedure must pass the message to the 
				// CallNextHookEx function without further processing and should return the value 
				// returned by CallNextHookEx. "
				// ...
				// "If nCode is greater than or equal to zero, and the hook procedure did not 
				// process the message, it is highly recommended that you call CallNextHookEx 
				// and return the value it returns;"
				#endregion
				return  CallNextHookEx(m_hDllKbdHook, nCode, wParam, lParam);
			}
			
			//
			// If you return a non-zero value, other key hooks will not process -- 
			// i.e. key processing will not happen as intended
			return lResult;
		}

		// 후킹 시작
		public static bool Hook(HookedKeyboardUserEventHandler callBackEventHandler)
		{
			bool bResult = true;
			m_hDllKbdHook = SetWindowsHookEx(
				(int)WH_KEYBOARD_LL, 
				m_LlKbEh, 
				Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]).ToInt32(), 
				0);
			
			if(m_hDllKbdHook == 0)
			{
				bResult = false;
			}
			// 외부에서 KeyboardHooker의 이벤트를 받을 수 있도록 이벤트 핸들러를 할당함
			KeyboardHooker.m_fpCallbkProc = callBackEventHandler;
			m_Hooked = bResult;
			return bResult;
		}

		// 후킹 중지
		public static void UnHook()
		{
			if(m_Hooked == true)
			{
				UnhookWindowsHookEx(m_hDllKbdHook);
				m_Hooked = false;
			}
		}

		/*******************************
		* +=
		* API Support Structures
		* -=
		******************************/ 
		#region KBDLLHOOKSTRUCT Documentation
		/// <summary>
		/// The KBDLLHOOKSTRUCT structure contains information about a low-level keyboard input event. 
		/// </summary>
		/// <remarks>
		/// <para>
		/// See <a href="ms-help://MS.VSCC/MS.MSDNVS/winui/hooks_0cxe.htm">KBDLLHOOKSTRUCT</a><BR/>
		/// </para>
		/// <para>
		/// <code>
		/// [C++]
		/// typedef struct KBDLLHOOKSTRUCT {
		///     DWORD     vkCode;
		///     DWORD     scanCode;
		///     DWORD     flags;
		///     DWORD     time;
		///     ULONG_PTR dwExtraInfo;
		///     ) KBDLLHOOKSTRUCT, *PKBDLLHOOKSTRUCT;
		/// </code>
		/// </para>
		/// </remarks>
		#endregion
		private struct KBDLLHOOKSTRUCT
		{
			#region vkCode
			/// <summary>
			/// Specifies a virtual-key code. The code must be a value in the range 1 to 254. 
			/// </summary>
			#endregion
			public int vkCode;
			#region scanCode
			/// <summary>
			/// Specifies a hardware scan code for the key. 
			/// </summary>
			#endregion
			public int scanCode;
			#region flags
			/// <summary>
			/// Specifies the extended-key flag, event-injected flag, context code, and transition-state flag.
			/// </summary>
			/// <remarks>
			/// For valid flag values and additional information, see <a href="ms-help://MS.VSCC/MS.MSDNVS/winui/hooks_0cxe.htm">MSDN Documentation for KBDLLHOOKSTRUCT</a>
			/// </remarks>
			#endregion
			public int flags;
			#region time
			/// <summary>
			/// Specifies the time stamp for this message. 
			/// </summary>
			#endregion
			public int time;
			#region dwExtraInfo
			/// <summary>
			/// Specifies extra information associated with the message. 
			/// </summary>
			#endregion
			public IntPtr dwExtraInfo;

			#region ToString()
			/// <summary>
			/// Creates a string representing the values of all the variables of an instance of this structure.
			/// </summary>
			/// <returns>A string</returns>
			#endregion
			public override string ToString()
			{
				string temp = "KBDLLHOOKSTRUCT\r\n";
				temp += "vkCode: " + vkCode.ToString() + "\r\n";
				temp += "scanCode: " + scanCode.ToString() + "\r\n";
				temp += "flags: " + flags.ToString() + "\r\n";
				temp += "time: " + time.ToString() + "\r\n";
				return temp;
			}
		}

		#region SetWindowsHookEx Documentation
		/// <summary>
		/// The SetWindowsHookEx function installs an application-defined hook procedure into a hook chain.
		/// You would install a hook procedure to monitor the system for certain types of events.
		/// These events are associated either with a specific thread or with all threads in the same
		/// desktop as the calling thread. 
		/// </summary>
		/// <remarks>
		/// <para>
		/// See <a href="ms-help://MS.VSCC/MS.MSDNVS/winui/hooks_7vaw.htm">SetWindowsHookEx</a><BR/>
		/// </para>
		/// <para>
		/// <code>
		/// [C++]
		///  HHOOK SetWindowsHookEx(
		///		int idHook,        // hook type
		///		HOOKPROC lpfn,     // hook procedure
		///		HINSTANCE hMod,    // handle to application instance
		///		DWORD dwThreadId   // thread identifier
		///		);
		/// </code>
		/// </para>
		/// </remarks>
		#endregion
		[DllImport(@"user32.dll", CharSet=CharSet.Auto)]
		private static extern long SetWindowsHookEx (	int idHook, 
			HookedKeyboardEventHandler lpfn, 
			long hmod,
			int dwThreadId);

		#region UnhookWindowsEx Documentation
		/// <summary>
		/// The UnhookWindowsHookEx function removes a hook procedure installed in a hook chain by the SetWindowsHookEx function. 
		/// </summary>
		/// <remarks>
		/// <para>
		/// See <a href="ms-help://MS.VSCC/MS.MSDNVS/winui/hooks_6fy0.htm">UnhookWindowsHookEx</a><BR/>
		/// </para>
		/// <para>
		/// <code>
		/// [C++]
		/// BOOL UnhookWindowsHookEx(
		///    HHOOK hhk   // handle to hook procedure
		///    );
		/// </code>
		/// </para>
		/// </remarks>
		#endregion
		[DllImport(@"user32.dll", CharSet=CharSet.Auto)]
		private static extern long UnhookWindowsHookEx	(long hHook);

		#region CallNextHookEx Documentation
		/// <summary>
		/// The CallNextHookEx function passes the hook information to the next hook procedure in the current hook chain. A hook procedure can call this function either before or after processing the hook information. 
		/// </summary>
		/// <remarks>
		/// <para>
		/// See <a href="ms-help://MS.VSCC/MS.MSDNVS/winui/hooks_57aw.htm">CallNextHookEx</a><BR/>
		/// </para>
		/// <para>
		/// <code>
		/// [C++]
		/// LRESULT CallNextHookEx(
		///    HHOOK hhk,      // handle to current hook
		///    int nCode,      // hook code passed to hook procedure
		///    WPARAM wParam,  // value passed to hook procedure
		///    LPARAM lParam   // value passed to hook procedure
		///    );
		/// </code>
		/// </para>
		/// </remarks>
		#endregion
		[DllImport(@"user32.dll", CharSet=CharSet.Auto)]
		private static extern long CallNextHookEx(		long hHook, 
			long nCode, 
			long wParam,
			IntPtr lParam);

		#region CopyMemory Documentation
		/// <summary>
		/// The CopyMemory function copies a block of memory from one location to another. 
		/// </summary>
		/// <remarks>
		/// <para>
		/// See <a href="ms-help://MS.VSCC/MS.MSDNVS/memory/memman_0z95.htm">CopyMemory</a><BR/>
		/// </para>
		/// <para>
		/// <code>
		/// [C++]
		/// VOID CopyMemory(
		///		PVOID Destination,   // copy destination
		///		CONST VOID* Source,  // memory block
		///		SIZE_T Length        // size of memory block
		///		);
		/// </code>
		/// </para>
		/// </remarks>
		#endregion
		[DllImport(@"kernel32.dll", CharSet=CharSet.Auto)]
		private static extern void CopyMemory(			ref KBDLLHOOKSTRUCT pDest, 
			IntPtr pSource, 
			long cb);

		#region GetAsyncKeyState
		/// <summary>
		/// The GetAsyncKeyState function determines whether a key is up or down at the time the function is called, and whether the key was pressed after a previous call to GetAsyncKeyState. 
		/// </summary>
		/// <remarks>
		/// <para>
		/// See <a href="ms-help://MS.VSCC/MS.MSDNVS/winui/keybinpt_1x0l.htm">GetAsyncKeyState</a><BR/>
		/// </para>
		/// <para>
		/// <code>
		/// [C++]
		///	SHORT GetAsyncKeyState(
		///		int vKey   // virtual-key code
		///		);
		/// </code>
		/// </para>
		/// </remarks>
		#endregion
		[DllImport(@"user32.dll", CharSet=CharSet.Auto)]
		private static extern uint GetAsyncKeyState(		int vKey	);

		#region GetForegroundWindow Documentation
		/// <summary>
		/// The GetForegroundWindow function returns a handle to the foreground window (the window with which the user is currently working). The system assigns a slightly higher priority to the thread that creates the foreground window than it does to other threads. 
		/// </summary>
		/// <remarks>
		/// <para>
		/// See <a href="ms-help://MS.VSCC/MS.MSDNVS/winui/windows_4f5j.htm">GetForegroundWindow</a><BR/>
		/// </para>
		/// <para>
		/// <code>
		/// [C++]
		/// HWND GetForegroundWindow(VOID);
		/// </code>
		/// </para>
		/// </remarks>
		#endregion
		[DllImport(@"user32.dll", CharSet=CharSet.Auto)]
		private static extern IntPtr GetForegroundWindow();
	}
}
