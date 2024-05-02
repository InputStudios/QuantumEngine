![logo](https://i.ibb.co/2hY32kK/bg-quantum-engine.png)

## Quantum Engine
Welcome to the Quantum Engine source code!

Quantum Engine provides a license agreement that will govern this and subsequent releases. Please review the [EULA here](https://sites.google.com/view/quantum-engine/) here to make sure you understand how these changes apply to you. By downloading or using Quantum Engine or otherwise indicating your acceptance of the License Agreement, you agree to be bound by its terms. If you do not or cannot agree to the terms of the license agreement, do not download or use Quantum Engine.

With the code in this repository, you can build the Quantum Editor for Windows, Mac, and Linux; compile Quantum Engine games for a variety of target platforms, including desktop, consoles, mobile, and embedded devices. Modify the code in any way you can imagine, and share your changes with others!

## Table of contents

- [Quantum Engine](#quantum-engine)
- [Table of contents](#table-of-contents)
- [Branches](#branches)
- [Getting up and running](#getting-up-and-running)
  - [Windows](#windows)
  - [Linux](#linux)
- [Licensing](#licensing)
- [Contributions](#contributions)
- [Community](#community)
- [Additional Notes](#additional-notes)

## Branches

We publish source for the engine in several branches:

- Numbered branches identify past and upcoming official **[release](https://github.com/InputStudios/QuantumEngine/tree/release)**, and the release branch always reflects the current official release. These are extensively tested by our QA team, so they make a great starting point for learning Quantum Engine and for making your own games. We work hard to make releases stable and reliable, and aim to publish a new release every few months.

- Most active development on Quantum Engine happens in the **[qe-main](https://github.com/InputStudios/QuantumEngine/tree/qe-master)** branch. This branch reflects the cutting edge of the engine and may be buggy — it may not even compile. We make it available for battle-hardened developers eager to test new features or work in lock-step with us.

  If you choose to work in this branch, be aware that it is likely to be ahead of the branches for the current official release and the next upcoming release. Therefore, content and code that you create to work with the qe-main branch may not be compatible with public releases until we create a new branch directly from qe-main for a future official release.

- Branches whose names contain dev, staging, and test are typically for internal Input Studios processes, and are rarely useful for end users.

Other short-lived branches may pop-up from time to time as we stabilize new releases or hotfixes.

## Getting up and running

- [Download the latest release](https://github.com/InputStudios/QuantumEngine/archive/0.0.4-preview.zip)
- Clone the repo: `git clone https://github.com/InputStudios/QuantumEngine.git`

The steps below take you through cloning your own private fork, then compiling and running the editor yourself:

### Windows
1.  Install **[GitHub Desktop for Windows](https://desktop.github.com/)** then **[fork and clone our repository](https://docs.github.com/en/get-started/exploring-projects-on-github/contributing-to-a-project)**.

    When GitHub Desktop is finished creating the fork, it asks, How are you planning to use this fork?. Select For my own purposes. This will help ensure that any changes you make stay local to your repository and avoid creating unintended pull requests. You'll still be able to make pull requests if you want to submit modifications that you make in your fork back to our repository.

    Other options:

    - To use Git from the command line, see the **[Setting up Git](https://docs.github.com/en/get-started/getting-started-with-git/set-up-git)** and **[Fork a Repo](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/working-with-forks/fork-a-repo)** articles.

    - If you'd prefer not to use Git, you can get the source with the Download ZIP button on the right. Note that the zip utility built in to Windows marks the contents of .zip files downloaded from the Internet as unsafe to execute, so right-click the .zip file and select Properties… and Unblock before decompressing it.

2. Install Visual Studio 2022.

    All desktop editions of Visual Studio 2022, 17.4 or later, 17.8 recommended, can build Quantum Engine, including **[Visual Studio Community](https://visualstudio.microsoft.com/vs/community/)**, which is free for small teams and individual developers.

    To install the correct components for Quantum Engine development, please see Setting Up Visual Studio.

3. Open your source folder in Windows Explorer and run Setup.bat. This will download binary content for the engine, install prerequisites, and set up Quantum file associations.

    On Windows 8, a warning from SmartScreen may appear. Click More info, then Run anyway to continue.

    A clean download of the engine binaries is currently 18+ GiB, which may take some time to complete. Subsequent runs will be much faster, as they only download new and updated content.

4. Run GenerateProjectFiles.bat to create project files for the engine. It should take less than a minute to complete.

5. Load the project into Visual Studio by double-clicking the new QuantumEngine.sln file.

6. Set your solution configuration to Development Editor and your solution platform to Win64, then right click the Quantum Engine target and select Build. It may take anywhere between 10 and 40 minutes to finish compiling, depending on your system specs.

7. After compiling finishes, you can run the editor from Visual Studio by setting your startup project to Quantum Engine and pressing F5 to start debugging.

### Linux

1. Install a visual Git client, then fork and clone our repository.

    Other options:

    - To use Git from the command line instead, see the Setting up Git and Fork a Repo articles.

    - If you'd prefer not to use Git, use the Download ZIP button on the right to get the source as a zip file.

2. Open your source folder and run Setup.sh to download binary content for the engine.

3. Both cross-compiling and native builds are supported.

    - Cross-compiling is handy for Windows developers who want to package a game for Linux with minimal hassle. It requires a cross-compiler toolchain to be installed. See the Linux cross-compiling page in the documentation.

    - Native compilation is discussed in a separate README and community wiki page.

## Licensing

Your access to and use of Quantum Engine on GitHub is governed by an End User License Agreement (EULA). For the latest terms and conditions, see the license and FAQ on the [Quantum Engine download page](https://inputstudios.ru/download). If you don't agree to the terms in your chosen EULA, as amended from time to time, you are not permitted to access or use Quantum Engine.

## Contributions
We welcome contributions to Quantum Engine development through [pull requests](https://github.com/InputStudios/QuantumEngine/pulls/) on GitHub.

We prefer to take pull requests in our active development branches, particularly for new features. Use the qe-main branch; Please make sure that all new code adheres to the Input Studios coding standards.

For more information on the process and expectations, see the documentation.

All contributions are governed by the terms of your EULA.

## Community

Get updates on Bootstrap's development and chat with the project maintainers and community members.

 - Follow [@inputstudios on Twitter](https://twitter.com/inputstudios).
 - Read and subscribe to [The Official Input Studios Blog](https://blog.inputstudios.ru/).
 - Ask questions and explore [our GitHub Discussions](https://github.com/orgs/InputStudios/discussions).
 - Discuss, ask questions, and more on [the community Discord](https://discord.gg/vSD27YNK) or [Quantum Engine subreddit](https://www.reddit.com/r/quantumengine/).

## Additional Notes
The first time you start the editor from a fresh source build, you may experience long load times. The engine is optimizing content for your platform and storing it in the [derived data cache](https://dev.inputstudios.com/documentation/en-us/quantum-engine/). This should only happen once.

Your private forks of the Quantum Engine code are associated with your GitHub account permissions. If you unsubscribe or switch GitHub user names, you'll need to create a new fork and upload your changes from the fresh copy.