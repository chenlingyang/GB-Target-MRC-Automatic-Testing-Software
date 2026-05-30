
' Values taken from MSDN.
Public Enum ItemSettings
    ' The item ...
    mfUnchecked = 0
    ' ... is not checked
    mfString = 0
    ' ... contains a string as label
    mfDisabled = 2
    ' ... is disabled
    mfGrayed = 1
    ' ... is grayed
    mfChecked = 8
    ' ... is checked
    mfPopup = 16
    ' ... Is a popup menu. Pass the
    '     menu handle of the popup
    '     menu into the ID parameter.
    mfBarBreak = 32
    ' ... is a bar break
    mfBreak = 64
    ' ... is a break
    mfByPosition = 1024
    ' ... is identified by the position
    mfByCommand = 0
    ' ... is identified by its ID
    mfSeparator = 2048
    ' ... is a seperator (String and
    '     ID parameters are ignored).
End Enum

Public Enum WindowMessages
    wmSysCommand = 274
End Enum

''' <summary>
''' A class that helps to manipulate the system menu
''' of a passed form.
'''
''' Written by Florian "nohero" Stinglmayr
''' </summary>
Public Class SystemMenu
    ' I havn't found any other solution than using plain old
    ' WinAPI to get what I want.
    ' If you need further information on these functions, their
    ' parameters, and their meanings, you should look them up in
    ' the MSDN.

    ' All parameters in the [DllImport] should be self explanatory.
    ' NOTICE: Use never stdcall as a calling convention, since Winapi
    ' is used.
    ' If the underlying structure changes, your program might cause
    ' errors that are hard to find.

    ' First, we need the GetSystemMenu() function.
    ' This function does not have an Unicode counterpart
    <Runtime.InteropServices.DllImport("USER32", EntryPoint:="GetSystemMenu", SetLastError:=True, CharSet:=CharSet.Unicode, ExactSpelling:=True, CallingConvention:=CallingConvention.Winapi)> _
    Private Shared Function apiGetSystemMenu(ByVal WindowHandle As IntPtr, ByVal bReset As Integer) As IntPtr
    End Function

    ' And we need the AppendMenu() function. Since .NET uses Unicode,
    ' we pick the unicode solution.
    <Runtime.InteropServices.DllImport("USER32", EntryPoint:="AppendMenuW", SetLastError:=True, CharSet:=CharSet.Unicode, ExactSpelling:=True, CallingConvention:=CallingConvention.Winapi)> _
    Private Shared Function apiAppendMenu(ByVal MenuHandle As IntPtr, ByVal Flags As Integer, ByVal NewID As Integer, ByVal Item As String) As Integer
    End Function

    ' And we also may need the InsertMenu() function.
    <Runtime.InteropServices.DllImport("USER32", EntryPoint:="InsertMenuW", SetLastError:=True, CharSet:=CharSet.Unicode, ExactSpelling:=True, CallingConvention:=CallingConvention.Winapi)> _
    Private Shared Function apiInsertMenu(ByVal hMenu As IntPtr, ByVal Position As Integer, ByVal Flags As Integer, ByVal NewId As Integer, ByVal Item As String) As Integer
    End Function

    Private m_SysMenu As IntPtr = IntPtr.Zero
    ' Handle to the System Menu
    Public Sub New()
    End Sub

    ' Insert a separator at the given position index starting at zero.
    Public Function InsertSeparator(ByVal Pos As Integer) As Boolean
        Return (InsertMenu(Pos, ItemSettings.mfSeparator Or ItemSettings.mfByPosition, 0, ""))
    End Function

    ' Simplified InsertMenu(), that assumes that Pos is a relative
    ' position index starting at zero
    Public Function InsertMenu(ByVal Pos As Integer, ByVal ID As Integer, ByVal Item As String) As Boolean
        Return (InsertMenu(Pos, ItemSettings.mfByPosition Or ItemSettings.mfString, ID, Item))
    End Function

    ' Insert a menu at the given position. The value of the position
    ' depends on the value of Flags. See the article for a detailed
    ' description.
    Public Function InsertMenu(ByVal Pos As Integer, ByVal Settings As ItemSettings, ByVal ID As Integer, ByVal Item As String) As Boolean
        Return (apiInsertMenu(m_SysMenu, Pos, DirectCast(Settings, Int32), ID, Item) = 0)
    End Function

    ' Appends a seperator
    Public Function AppendSeparator() As Boolean
        Return AppendMenu(0, "", ItemSettings.mfSeparator)
    End Function

    ' This uses the ItemSettings.mfString as default value
    Public Function AppendMenu(ByVal ID As Integer, ByVal Item As String) As Boolean
        Return AppendMenu(ID, Item, ItemSettings.mfString)
    End Function
    ' Superseded function.
    Public Function AppendMenu(ByVal ID As Integer, ByVal Item As String, ByVal Settings As ItemSettings) As Boolean
        Return (apiAppendMenu(m_SysMenu, CInt(Settings), ID, Item) = 0)
    End Function

    ' Retrieves a new object from a Form object
    Public Shared Function FromForm(ByVal Frm As Form) As SystemMenu
        Dim cSysMenu As New SystemMenu()
        cSysMenu.m_SysMenu = apiGetSystemMenu(Frm.Handle, 0)
        Return cSysMenu
    End Function

    ' Reset's the window menu to it's default
    Public Shared Sub ResetSystemMenu(ByVal Frm As Form)
        apiGetSystemMenu(Frm.Handle, 1)
    End Sub

    ' Checks if an ID for a new system menu item is OK or not
    Public Shared Function VerifyItemID(ByVal ID As Integer) As Boolean
        Return CBool((ID < 61440 AndAlso ID > 0))
    End Function
End Class


