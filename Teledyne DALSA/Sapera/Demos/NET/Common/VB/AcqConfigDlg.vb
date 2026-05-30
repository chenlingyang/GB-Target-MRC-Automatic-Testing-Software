
Public Class AcqConfigDlg

    Private m_ConfigFileEnable As Boolean
    Private m_configFileAvailable As Boolean
    Private m_currentConfigDir As String
    Private m_currentConfigFileName As String
    Private m_currentConfigFileIndex As Integer
    Private m_ProductDir As String = ""
    Private m_ServerName As String = ""
    Private m_ConfigFile As String = ""
    Private m_ResourceIndex As Integer = 0
    Private m_ServerCategory As ServerCategory
    Private m_init As Boolean = False
    Private ccffiles As ArrayList = New ArrayList()

    <DllImport("kernel32")> _
 Private Shared Function GetPrivateProfileString(ByVal section As String, ByVal key As String, ByVal def As String, ByVal retVal As StringBuilder, ByVal size As Integer, ByVal filePath As String) As Integer
    End Function

    Shared ConfigKeyName As String = "Camera Name"
    Shared CompanyKeyName As String = "Company Name"
    Shared ModelKeyName As String = "Model Name"
    Shared VicName As String = "Vic Name"
    Shared Acquisition_Default_folder As String = "\CamFiles\User"

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub New(ByVal serverCategory As ServerCategory)
        m_ServerCategory = serverCategory
        ' Load parameters from registry
        LoadSettings()
        InitializeComponent()
    End Sub


    Public Sub New(ByVal loc As SapLocation, ByVal productDir As String, ByVal serverCategory As ServerCategory)
        m_ProductDir = productDir
        m_ServerCategory = serverCategory

        If loc IsNot Nothing Then
            m_ServerName = loc.ServerName
            m_ResourceIndex = loc.ServerIndex
        End If
        ' Load parameters from registry
        LoadSettings()
        InitializeComponent()
    End Sub


    Private Function InitServerCombo() As Boolean
        comboBox_Server.Items.Clear()
        For i As Integer = 0 To SapManager.GetServerCount() - 1
            ' Does this server support "Acq" (frame-grabber) or "AcqDevice" (camera)?
            Dim bAcq As Boolean = (m_ServerCategory = ServerCategory.ServerAcq OrElse m_ServerCategory = ServerCategory.ServerAll) AndAlso (SapManager.GetResourceCount(i, SapManager.ResourceType.Acq) > 0)
            Dim bAcqDevice As Boolean = (m_ServerCategory = ServerCategory.ServerAcqDevice OrElse m_ServerCategory = ServerCategory.ServerAll) AndAlso _
                (SapManager.GetResourceCount(i, SapManager.ResourceType.AcqDevice) > 0 AndAlso _
                 SapManager.GetResourceCount(i, SapManager.ResourceType.Acq) = 0)

            If bAcq Then
                Dim serverName As String = SapManager.GetServerName(i)
                comboBox_Server.Items.Add(New MyListBoxItem(serverName, True))
            ElseIf bAcqDevice Then
                Dim serverName As String = SapManager.GetServerName(i)
                If (Not serverName.Contains("CameraLink_") AndAlso Not serverName.Contains("LP1") AndAlso Not serverName.Contains("LP2")) Then
                    ComboBox_Server.Items.Add(New MyListBoxItem(serverName, False))
                End If
            End If
        Next
        If comboBox_Server.Items.Count <= 0 Then
            Return False
        Else
         If m_ServerName = "" Or ComboBox_Server.FindString(m_ServerName, 0) = -1 Then
            ComboBox_Server.SelectedIndex = 0
            m_ServerName = ComboBox_Server.SelectedItem.ToString()
         Else
            ComboBox_Server.SelectedIndex = ComboBox_Server.FindString(m_ServerName, 0)
         End If

            InitResourceCombo()
            Return True
        End If
    End Function

    Private Sub InitResourceCombo()
        comboBox_Device.Items.Clear()
        Dim i As Integer = 0
        Dim selectedItem As Object = comboBox_Server.SelectedItem
        ' Add "Acq" resources (cameras) to combo
        For i = 0 To SapManager.GetResourceCount(selectedItem.ToString(), SapManager.ResourceType.Acq) - 1
            Dim resourceName As String = SapManager.GetResourceName(selectedItem.ToString(), SapManager.ResourceType.Acq, i)
            If SapManager.IsResourceAvailable(selectedItem.ToString(), SapManager.ResourceType.Acq, i) Then
                ComboBox_Device.Items.Add(resourceName)
                If i = m_ResourceIndex Then
                    ComboBox_Device.SelectedItem = resourceName
                End If
            Else
                ComboBox_Device.Items.Add("Not Available - Resource is Use")
                ComboBox_Device.SelectedIndex = 0
            End If
        Next

        ' Add "AcqDevice" resources (cameras) to combo
        If SapManager.GetResourceCount(selectedItem.ToString(), SapManager.ResourceType.Acq) = 0 Then
            For i = 0 To SapManager.GetResourceCount(selectedItem.ToString(), SapManager.ResourceType.AcqDevice) - 1
                Dim resourceName As String
                resourceName = SapManager.GetResourceName(selectedItem.ToString(), SapManager.ResourceType.AcqDevice, i)
                ComboBox_Device.Items.Add(resourceName)
                If i = m_ResourceIndex Then
                    ComboBox_Device.SelectedItem = resourceName
                End If
            Next
        End If
        m_ResourceIndex = ComboBox_Device.SelectedIndex
    End Sub

    Private Sub UpdateCurrentDir(ByVal newCurrentDir As String)
        If String.Compare(newCurrentDir, m_currentConfigDir, StringComparison.Ordinal) <> 0 Then
            TextBox_configfile.Text = newCurrentDir
            m_currentConfigDir = newCurrentDir
        End If
    End Sub

    Private Sub LoadSettings()
        Dim KeyPath As String = "Software\Teledyne DALSA\" + Application.ProductName + "\SapAcquisition"
        Dim RegKey As RegistryKey = Registry.CurrentUser.OpenSubKey(KeyPath)
        If RegKey IsNot Nothing Then
            ' Read location (server and resource) and file name
            m_ServerName = RegKey.GetValue("Server", "").ToString()
            m_ResourceIndex = CInt(RegKey.GetValue("Resource", 0))
            If File.Exists(RegKey.GetValue("ConfigFile", "").ToString()) Then
                m_ConfigFile = RegKey.GetValue("ConfigFile", "").ToString()
            End If
        End If
    End Sub

    Private Sub SaveSettings()
        Dim KeyPath As String = "Software\Teledyne DALSA\" + Application.ProductName + "\SapAcquisition"
        Dim RegKey As RegistryKey = Registry.CurrentUser.CreateSubKey(KeyPath)
        ' Write config file name and location (server and resource)
        RegKey.SetValue("Server", m_ServerName)
        RegKey.SetValue("ConfigFile", m_ConfigFile)
        RegKey.SetValue("Resource", m_ResourceIndex)
    End Sub

    Private Sub UpdateNames()
        'Delete ccf name file list 
        ccffiles.Clear()

      Dim currentDir As String = m_currentConfigDir
      Dim keyName As String = ConfigKeyName
      Dim curServerName As String = ComboBox_Server.SelectedItem.ToString()
      m_ResourceIndex = ComboBox_Device.SelectedIndex
      m_ServerName = curServerName
      curServerName = curServerName.Substring(0, curServerName.Length - 2)

      Dim dir As New DirectoryInfo(currentDir)
      If dir.Exists Then
         Dim ccffileInfo As FileInfo() = dir.GetFiles("*.ccf")
         ComboBox_configfile.Items.Clear()

         For Each f As FileInfo In ccffileInfo
            Dim filePath As String = m_currentConfigDir + "\" + f.Name
            Dim tempServerName As New StringBuilder(512)
            Dim sbCameraName As New StringBuilder(512)
            Dim sbCompanyName As New StringBuilder(512)
            Dim sbModelName As New StringBuilder(512)
            Dim sbVicName As New StringBuilder(512)
            Dim companyName As String = ""
            Dim modelName As String = ""
            Dim cameraDesc As String = ""

            GetPrivateProfileString("Board", "Server name", "Unknow", tempServerName, 512, filePath)

            ' Check if the current configuration file has been created for the current server 
                If String.Compare(curServerName, tempServerName.ToString(), StringComparison.OrdinalIgnoreCase) <> 0 Then
                    Continue For
                End If
            ' Add ccf File name
            ccffiles.Add(f.Name)

            GetPrivateProfileString("General", keyName, "Unknown", sbCameraName, 512, filePath)
            GetPrivateProfileString("General", CompanyKeyName, "", sbCompanyName, 512, filePath)
            GetPrivateProfileString("General", ModelKeyName, "", sbModelName, 512, filePath)
            GetPrivateProfileString("General", VicName, "", sbVicName, 512, filePath)

            If sbCompanyName.ToString().Length <> 0 AndAlso sbModelName.ToString().Length <> 0 Then
               companyName = sbCompanyName.ToString() + ", "
            End If
            If sbModelName.ToString().Length <> 0 AndAlso sbCameraName.ToString().Length <> 0 Then
               modelName = sbModelName.ToString() + ", "
            End If

            cameraDesc = companyName + modelName + sbCameraName.ToString() + " - " + sbVicName.ToString()


            Dim item As New MyListBoxItem(cameraDesc, True)
            ComboBox_configfile.Items.Add(item)
         Next
      End If
     

      If ComboBox_configfile.Items.Count <> 0 Then
         m_configFileAvailable = True
         Dim newFileIndex As Integer = 0

         ' Try to find the current camera file selected
         For i As Integer = 0 To ccffiles.Count - 1
            Dim currentccf As String = DirectCast(ccffiles(i), String)
                If String.Compare(m_currentConfigFileName, currentccf, StringComparison.Ordinal) = 0 Then
               newFileIndex = i
            End If
         Next

         ComboBox_configfile.SelectedIndex = newFileIndex
      Else
         m_configFileAvailable = False
      End If
      UpdateBoxAvailability()
   End Sub


    Private Sub UpdateBoxAvailability()
        ' Is config file is required by this type of server?
        Dim item As MyListBoxItem = DirectCast(comboBox_Server.SelectedItem, MyListBoxItem)
        Dim configFileRequired As Boolean = item.ItemData
        checkBox_configfile.Enabled = Not configFileRequired

        If configFileRequired Then
            m_ConfigFileEnable = configFileRequired
            checkBox_configfile.Checked = configFileRequired
        Else
            m_ConfigFileEnable = checkBox_configfile.Checked
        End If

        comboBox_configfile.Enabled = (m_ConfigFileEnable AndAlso m_configFileAvailable)
        textBox_configfile.Enabled = (m_ConfigFileEnable)
        button_browse.Enabled = (m_ConfigFileEnable)
        button_ok.Enabled = Not m_ConfigFileEnable OrElse m_configFileAvailable
    End Sub

    Private Sub SetDirectories()
        Dim productDirectory As String = ""

        If Not String.IsNullOrEmpty(m_ProductDir) Then
            productDirectory = Environment.GetEnvironmentVariable(m_ProductDir)
        End If

        Dim saperaDir As String = Environment.GetEnvironmentVariable("SAPERADIR")

        If m_ConfigFile.Length <> 0 Then
            m_currentConfigDir = System.IO.Path.GetDirectoryName(m_ConfigFile)
            m_currentConfigFileName = System.IO.Path.GetFileName(m_ConfigFile)
            textBox_configfile.Text = m_currentConfigDir
        Else
            m_currentConfigFileName = ""
            If Not String.IsNullOrEmpty(productDirectory) Then
                m_currentConfigDir = productDirectory
            ElseIf saperaDir.Length <> 0 Then
                m_currentConfigDir = saperaDir + Acquisition_Default_folder
            End If
            textBox_configfile.Text = m_currentConfigDir
        End If
    End Sub

    Private Sub AcqConfigDlg_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Initialize Servers Combo
        If Not InitServerCombo() Then
            Me.Close()
        Else
            ' Initialize directories
            SetDirectories()
            ' Scan all files in the current directory and fill the list box
            UpdateNames()
        End If
        m_init = True
    End Sub

    Private Sub ComboBox_Server_SelectedIndexChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox_Server.SelectedIndexChanged
        If m_init Then
            InitResourceCombo()
            UpdateNames()
            UpdateBoxAvailability()
        End If
    End Sub

    Private Sub ComboBox_Device_SelectedIndexChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox_Device.SelectedIndexChanged
        If m_init Then
            UpdateNames()
        End If
    End Sub

    Private Sub CheckBox_Configfile_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox_Configfile.CheckedChanged
        m_ConfigFileEnable = CheckBox_Configfile.Checked
        UpdateNames()
    End Sub

    Private Sub ComboBox_configfile_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox_configfile.SelectedIndexChanged
        m_currentConfigFileIndex = ComboBox_configfile.SelectedIndex
        m_currentConfigFileName = DirectCast(ccffiles(m_currentConfigFileIndex), String)
    End Sub

    Private Sub Button_Browse_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Browse.Click
        Me.FolderBrowserDialog1.Description = "Select a directory."
        Me.FolderBrowserDialog1.SelectedPath = m_currentConfigDir
        If Me.FolderBrowserDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            If Me.FolderBrowserDialog1.SelectedPath <> String.Empty Then
                UpdateCurrentDir(FolderBrowserDialog1.SelectedPath)
            End If
            Dim countItems As Integer = ComboBox_configfile.Items.Count
            ComboBox_configfile.Items.Clear()
            UpdateNames()
        End If
    End Sub


    Private Sub Button_OK_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_OK.Click
        If CheckBox_Configfile.Checked Then
            m_ConfigFile = m_currentConfigDir + "\" + m_currentConfigFileName
        Else
            m_ConfigFile = ""
        End If

        If m_ServerCategory = ServerCategory.ServerAll Then
            If SapManager.GetResourceCount(New SapLocation(m_ServerName, m_ResourceIndex), SapManager.ResourceType.Acq) > 0 Then
                m_ServerCategory = ServerCategory.ServerAcq
            ElseIf SapManager.GetResourceCount(New SapLocation(m_ServerName, m_ResourceIndex), SapManager.ResourceType.AcqDevice) > 0 Then
                m_ServerCategory = ServerCategory.ServerAcqDevice
            End If
        End If
        SaveSettings()
    End Sub


    ''' <summary>
    ''' Property of the form 
    ''' </summary>
    Public ReadOnly Property ConfigFile() As String
        Get
            If m_ConfigFileEnable Then
                Return m_ConfigFile
            Else
                Return ""
            End If
        End Get
    End Property

    Public ReadOnly Property ServCategory() As ServerCategory
        Get
            Return m_ServerCategory
        End Get
   End Property


    Public ReadOnly Property ServerLocation() As SapLocation
        Get
            Return New SapLocation(m_ServerName, m_ResourceIndex)
        End Get
    End Property

    ''' <summary>
    ''' Enum of server category
    ''' </summary>

    Public Enum ServerCategory
        ServerAll
        ServerAcq
        ServerAcqDevice
    End Enum

    ''' <summary>
    ''' Class to handle listbox item
    ''' </summary>
    Class MyListBoxItem
        Public Sub New(ByVal Text As String)
            Camdesc = Text
            m_itemData = False
        End Sub

        Public Sub New(ByVal Text As String, ByVal ItemData As Boolean)
            Camdesc = Text
            m_itemData = ItemData
        End Sub

        Public Property ItemData() As Boolean
            Get
                Return m_itemData
            End Get
            Set(ByVal value As Boolean)
                m_itemData = value
            End Set
        End Property

        Public Overloads Overrides Function ToString() As String
            Return Camdesc
        End Function

        Private Camdesc As String
        Private m_itemData As Boolean
    End Class
End Class

