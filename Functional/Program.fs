module Program

open System
open Utilities
open Book
open Reports
open Member

let rec mainMenu books members borrowedBooks =
    
    printfn "\nLibrary Management System"
    printfn "1. Add Book"
    printfn "2. Remove Book"
    printfn "3. Update Book"
    printfn "4. Search Books"
    printfn "5. Register Member"
    printfn "6. View Member"
    printfn "7. Update Member"
    printfn "8. Borrow Book"
    printfn "9. Return Book"
    printfn "10. List Available Books"
    printfn "11. Your Borrowing Books"
    printfn "12. Exit"
    printf "Enter your choice: "
    
    let choice = Console.ReadLine()
    match choice with
    | "1" ->
        printf "Enter Title: "
        let title = Console.ReadLine()
        printf "Enter Author: "
        let author = Console.ReadLine()
        printf "Enter Genre: "
        let genre = Console.ReadLine()
        Book.addBook title author genre
        mainMenu books members borrowedBooks
    | "2" ->
        printf "Enter Title: "
        let title = Console.ReadLine()
        Book.removeBook title
        mainMenu books members borrowedBooks
    | "3" ->
        printf "Enter Title: "
        let title = Console.ReadLine()
        printf "Enter New Title: "
        let newTitle = Console.ReadLine()
        printf "Enter New Author: "
        let newAuthor = Console.ReadLine()
        printf "Enter New Genre: "
        let newGenre = Console.ReadLine()
        Book.updateBook title newTitle newAuthor newGenre
        mainMenu books members borrowedBooks
    | "4" ->
        printf "Enter Search Term (Title, Author, or Genre): "
        let searchTerm = Console.ReadLine()
        Book.searchBooks searchTerm
        mainMenu books members borrowedBooks
    | "5" ->
        printf "Enter Name: "
        let name = Console.ReadLine()
        printf "Enter Member ID: "
        let memberId = Console.ReadLine()
        Member.registerMember name memberId
        mainMenu books members borrowedBooks
    | "6" ->
        printf "Enter Member ID: "
        let memberId = Console.ReadLine()
        Member.viewMember memberId
        mainMenu books members borrowedBooks
    | "7" ->
        printf "Enter Member ID: "
        let memberId = Console.ReadLine()
        printf "Enter New Name: "
        let newName = Console.ReadLine()
        Member.updateMember memberId newName
        mainMenu books members borrowedBooks
    | "8" ->
        printf "Enter Title: "
        let title = Console.ReadLine()
        printf "Enter Member ID: "
        let memberId = Console.ReadLine()
        Book.borrowBook title memberId
        mainMenu books members borrowedBooks
    | "9" ->
        printf "Enter Title: "
        let title = Console.ReadLine()
        Book.returnBook title
        mainMenu books members borrowedBooks
    | "10" ->
        Reports.listAvailableBooks()
        mainMenu books members borrowedBooks
    | "11" ->
        printf "Enter Member ID: "
        let memberId = Console.ReadLine()
        Reports.yourBorrowingBooks memberId
        mainMenu books members borrowedBooks
    | "12" ->
        printfn "Exiting Library Management System. Goodbye!"
        ()
    | _ ->
        printfn "Invalid choice. Please try again."
        mainMenu books members borrowedBooks

[<EntryPoint>]
let mainEntry argv =
    printfn "Functional Library Management System"
    initializeFile booksFilePath
    initializeFile membersFilePath
    initializeFile borrowedBooksFilePath

    mainMenu booksFilePath membersFilePath borrowedBooksFilePath
    0