# <center>MVC Library</center>

## Table of Contents
1. [How to use](#how-to-use)
2. [Structure](#structure)
3. [Administrator](#administrator)
4. [Features](#features)
    1. [Add books](#add-books)
    2. [Add categories](#add-categories)
    3. [Manage books](#manage-books)
    4. [Lend books](#lend-books)




<br/><br/>

## How to use
Type in the following command in the terminal to start the server:
```bash
dotnet run
```
Then read the instructions on the console. There will be provided a `localhost` link. Copy it and paste it in your browser.


## Structure
### Register
In order to use the site u need to register first. To do so, click on the register button on the top right corner of the page. Then fill in the form and click on the `submit` button.  
After that a site admin will have to approve your account and you will be able to login. 

### Login 
To login, click on the login button. Then fill in the form and click on the `submit` button. A message will appear whether the login was successful or not. After that you will gain access to all of the features that were granted to you.

### Administrator
The administrator has special privileges. He can add, edit and delete access to any feature for any of the registered users with lower privileges.

### Features
#### Add books
The administrator can add books to the library. To do so, he needs to click on the `Add book` button. Then he needs to fill in the form and click on the `submit` button. In the last field of the form you will be able to choose a category. In order to add one you need to click on the `Add category` button. 
#### Add categories
To add categories, the administrator can click on the "Add category" button in the book addition form. A new form will appear where the administrator can enter the name of the category and click on the "Submit" button to add it to the list of categories.

#### Manage books
The administrator has the ability to manage the books in the library. This includes editing book information such as title, author, and category, as well as deleting books from the library. To manage a book, the administrator can navigate to the book's details page and click on the "Edit" or "Delete" button.

#### Lend books
The lending feature allows the administrator to lend books to registered users. To lend a book, the administrator can navigate to the book's details page and click on the "Lend" button. A form will appear where the administrator can select the user to whom the book will be lent and enter the due date. After filling in the required information, the administrator can click on the "Submit" button to complete the lending process.

### Please note that only registered users with appropriate privileges can access these features.


