# Interview Take Home Test Submission for JR Smith

I chose to use the first question from the prompt for my interview take home test. I opted to make my solution more data driven than the promp called for. Since the prompt is really a touchpoint for a conversation, I thought making it data driven would make for an interesting starting point since I've been doing a lot of that at work.

## Build Notes

I used VS Code v1.44. I ran this on Windows 10. I use .NET Core v3.1.3. I used the C# plugin for VS Code from Microsoft v1.21.17. `dotnet --version` outputs 3.1.201.

I purposefully did not include my .vscode config files as your .NET install may be different. Assuming you have .NET and VS Code installed, please do the following:

1. git clone https://github.com/jrsmith17/speedy-loop-interview.git
2. Open VS Code
    - Install C# module from Marketplace (More details here: [https://code.visualstudio.com/docs/editor/extension-gallery])
    - Open Folder from File Menu, select the `speedy-loop-interview` directory that you cloned.
    - VS Code should prompt you for which environment you wish to run. Please select .NET Core (was at the top of the list for me)
3. VS Code will take a moment to install plugins and generally setup your environment.
4. Once done, hitting either F5 or `Run > Start Debugging` should start the process. The output window should show you output that matches the prompt.

## Assumptions Made

1. The prompt didn't specify what form the input should take. It did say a directed graph, but the chances of my directed graph binary blob being the same as yours were slim so I decided to make a simple text file input.
2. Wasn't specified if the distance was going to be less than 10, so I made my validation code check for multiple digits.
3. I considered providing error codes for some of my try catch statements, but the prompt said not to gold plate my solution. Since I was only using insertion attemp results to debug my results, a simple bool was fine.
4. I didn't want to add dependencies outside the standard library. I couldn't find a native way to read in command line options to override file names for my input files. I filed that under "not gold plating it".
5. I considered doing unit tests. I saw the Microsoft Docs relied on a VisualStudio library. I couldn't guarantee that this wouldn't be run under a mono environment and I wasn't sure if that library would be included. Chalked it up to "not gold plating it" plus not using the language for a long time.

## Things I Would Do In A Real Coding Environment

1. Add unit tests. Invesigate mono vs VS question in terms of library support. Since I built my submission around text input files, it would be fairly simply to generate some known good or known bad inputs.
2. Test cross platform. Only tested on my Windows machine.
3. Run a fuzzer against my program for extra checks against failing gracefully.
4. Investigate how C# supports automatic dependency installation, if at all. (i.e. like package.json in the Nodejs world)
5. Command line options to change the input/output txt files.
6. Setup code auto formatter
7. Setup code linter
8. For recursive functions, consider passing around one object instead of many parameters. It's just on the cusp of where I would normally make the switch.
9. Consider merging trip length functions. It was late when I reread the prompt and caught the difference between max and exact stop length. Decided to keep it separate.
10. Add proper documentation
11. Consider making base DirectionalGraph class and then a prompt specific class to extend it.

## Process

I haven't coded in C# in almost a decade. Given that, I spent an hour or so refamiliarizing myself with the language by reading in a file, writing some regex matching, etc. Once I felt familiar enough to get started, I worked on visualizing the problem.

I drew out the solution on paper. I find that doing so for a simple case helps me work through the most appropriate solution. More specifically, I drew out a directed graph, but I also drew out an array - since that's how many graph implementations are actually stored. Doing this helped me realize that I would want to work through the following cases when processing input lines from my text file:

1. Completely empty graph:
    - Add one town with a list with one route.
    - Add one town with an empty list of routes.
2. Town doesn't exist:
    - Add new town with empty list of routes. 
        - If connected town exists, add route to list.
        - If connected town does not exist, add route to list and add one town with an empty list of routes.
3. Town does exist:
    a. Connected town exists but route not previously used. Add route to list of routes.
    b. Connected town does not exist. Add town with empty list of routes and add to list of routes for current town.
4. Town and route combination already exists:
    - Spec says a given town + route combination will only exist once. Either do nothing or output to a duplicates file.

I then wrote code to validate my input files. I usually it's a touch awkward to call them input/output files, but that's what the prompt called them so I decided to stick with that. Turns out my practice with Regex came in handy. From there, I coded up a few iterations of my DirectedGraph class. I considered using C#'s unsafe access to pointers, but since my C# is rusty I'm not fully aware of the consequences of unsafe in this context. I ultimately ended up using a Dictionary based implementation. I also made a debug print function so I could check that my internal representation matched my drawing.

Next I wrote more validation code, this time for my output file. Once again, I used a very simple Regex to check the quality of the incoming data. Lastly, I coded up the what the prompt asked for under the Output section.