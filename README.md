# Phi Module Packer
A generic pre-processor for [Phi style modules](https://github.com/MinecraftPhi/phi.core/blob/develop/docs/module_structure.md)

## Packer-Common
Class library that loads the module, downloads the required dependencies, loads the associated pre-processors (using a callback to download), and runs the pre-processors.

Written in C# on dotnet core

## Packer-Downloader
Class library that uses Packer-Common with a callback that can download pre-processors.

Written in C# on dotnet core

## Commandline Tool
Uses Packer-Downloader.

Written in C# on dotnet core

Tools:
- `new module <name>`  
  Scaffolds a new module called `<name>`
- `new preprocessor <name>`  
  Scaffolds a new C# solution/project for a new preprocessor
- `build`  
  Builds the module in the current folder
- `watch`  
  Continuously watches for file changes in the module in the current folder and builds automatically (with some debouncing)

## Pre-processor API
Class library containing the interfaces for pre-processors

Written in C# on dotnet core

## Website Backend
Uses Packer-Common with a callback that can only use verified pre-processors.

Written in C# on dotnet core using ASP.net core

### API Endpoints (incomplete)
- pre-processor
    - download
    - upload to be verified
    - privileged only:
        - download source
        - upload compiled and verify
    - view
    - search
    - list
- module
    - search
    - list
    - view
    - upload
    - download distributable
    - download raw

### Pre-processor Verification Process
When uploading a pre-processor (even for new versions of existing ones) it is first put into the "PENDING VERIFICATION" state.  
The source code must be supplied, either by directly uploading it or by a git URL (recommended).  
A zip of the compiled pre-processor (including all dependencies) may optionally also be supplied. In which case, the pre-processor will be available for manual download, but will not be available for use on the website or for automatic download in the Packer-Downloader.

Specially trusted verifier individuals will be notified of the new pre-processor version. When a verifier begins checking the source code the pre-processor will be put into the "VERIFICATION IN-PROGRESS" state.
If the verifier(s) decide that the pre-processor is safe to run they will compile it, zip the result (including dependencies), and upload the zip. This will put the pre-processor into the "VERIFIED" state and it will then be available to be run on the website and for automatic download in the Packer-Downloader

If the verifier(s) decide that the pre-processor is unsafe it will be put into the "COMPROMISED" state. This will hide the pre-processor from public view, only verifiers and the owner of the pre-processor will be able to see it.  
The owner of the pre-processor will then be notified of the problem, with an explanation of why it was denied.

## Website Frontend
Website frontend. Made using angular, hosted on nginx independent of website backend.

If the backend goes down the frontend will continue to load but will show that backend is down.

If the frontend goes down the backend API will continue to function and not interupt the Commandline Tool or other tools that use the Packer-Downloader