#define AppName "DevTools"
#define AppPublisher "DevTools"
#define AppURL "https://github.com/user/devtools"
#define AppExeName "DevTools.exe"
#define IconPath "..\Resources\Images\logo.ico"

[Setup]
AppId={{8B5F3C7A-1D2E-4F6B-9C3A-5E7D8F1A2B3C}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL={#AppURL}
AppSupportURL={#AppURL}
AppUpdatesURL={#AppURL}
#if Architecture == "x64" || Architecture == "arm64"
DefaultDirName={autopf64}\{#AppName}
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64
#else
DefaultDirName={autopf32}\{#AppName}
#endif
DefaultGroupName={#AppName}
AllowNoIcons=yes
OutputDir={#OutputDir}
OutputBaseFilename={#AppName}-{#Architecture}-Setup-{#AppVersion}
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
UninstallDisplayIcon={app}\{#AppExeName}
UninstallDisplayName={#AppName}
VersionInfoVersion={#AppVersion}
VersionInfoCompany={#AppPublisher}
VersionInfoDescription={#AppName} Installer
VersionInfoCopyright=Copyright (C) 2024
VersionInfoProductName={#AppName}
VersionInfoProductVersion={#AppVersion}
SetupIconFile={#IconPath}
Uninstallable=yes
UsePreviousAppDir=yes
DisableDirPage=no
DisableProgramGroupPage=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#SourceDir}\{#AppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{group}\{cm:ProgramOnTheWeb,{#AppName}}"; Filename: "{#AppURL}"
Name: "{group}\{cm:UninstallProgram,{#AppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: filesandordirs; Name: "{app}"
