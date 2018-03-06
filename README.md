# Ravel
Ah shit here we go


## Naming Conventions
### Tech
Language - C#

Variables - camelCase

**Use good style plz**

Unity Version - 2018

### Modeling
Those of you who are dealing with modeling, please add to this. 

---
#Credits

**Please note things we should credit here** 

* Greer - Hannah Cornish

---
# The Heck Is a Git
[XKCD](https://xkcd.com/1597/)

## Workflow Overview
We all work on our own branches.
When you have a thing you want to include in the project, load it into the global version of your branch, and then create a [pull request](https://github.com/Fireheart182/Ravel/pulls) (PR) with **master** as the base and **\<your name\>** as the compare. 
I've protected the **master** branch, which means that you can't make changes directly to **master** and instead have to create a PR. Hopefully, that'll protect the whole project from a mistake in using git.

On your own local branch, you do work as you normally would, treating the github repo like you would any folder on your computer. As you work, you make commits, which act like steps in production. I recommend making individual commits for each step of your work, as that will make recovery easier if things go wrong.
## Setting up
To create a local copy of the github repo on your computer:
    
* Open Terminal (or Powershell)
* Navigate to the directory you want to store the local copy in
    * `cd` to change directory
    * `mkdir` to create a new directory
    * `ls` to list out the files in your current directory
* `git clone https://github.com/Fireheart182/Ravel.git` to create a local copy right here. This will create a new local directoy called **Ravel** that is connected to the global github version.
* I've create different branches for each person, so to make sure you're working on the right branch, run the command `git checkout -b <your name>`. This will create a local branch **\<your name\>**. Then, to link your local branch with the global branch, run `git push --set-upstream origin <your name>`
* If you want to see a list of all the branches, you can run `git branch -a`. I've protected the **master** branch, so if you do work there, you're going to have a bad time. You should see a little astrisk next to your name, to indicate you are currently working on that branch.

## Working
#### Starting a new thing
* verify you're on your local branch with `git branch`
* `git pull origin master` to pull down anything else that has been changed since you last did work

#### While Working
* Do your thing on your local branch
* `git add [files]`, where [files] is a list of the files you modified seperated by spaces (`myFile1 myFile2 ...`). Alternatively, you can add in all changed files with `git add -A`. I don't recommend doing that, it can lead to weird nasty errors, particularly when we're dealing with a Unity project or Maya files.
* `git commit` Will open a dialogue where you can type a commit message in vim, which is a Terminal-based text editor (and a pain in the ass). Press `i` to get to insert mode, navigate using arrow keys, and then save and exit by pressing `esc`, then typing `:wq`, then pressing `return`.

#### Publish to Global
* `git pull origin master`
* fix any collisions
    * If there were any collisions, then after fixing them:
        * `git add [files]`
        * `git commit`
* `git push`

#### Integrate to Master
* create a [pull request](https://github.com/Fireheart182/Ravel/pulls)
