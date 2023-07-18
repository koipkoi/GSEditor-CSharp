@echo off

@rd /s/q .idea
@rd /s/q .vs
@rd /s/q .vscode

@rd /s/q Publish

@rd /s/q GSEditor\bin
@rd /s/q GSEditor\obj

@rd /s/q GSEditor.Common\bin
@rd /s/q GSEditor.Common\obj

@rd /s/q GSEditor.Contract\bin
@rd /s/q GSEditor.Contract\obj

@rd /s/q GSEditor.Models\bin
@rd /s/q GSEditor.Models\obj

@rd /s/q GSEditor.Services\bin
@rd /s/q GSEditor.Services\obj

@del /Q /F /S "*.user"
