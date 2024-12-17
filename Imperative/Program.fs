module Program

open System
open Utilities
open Book
open Reports
open Member

[<EntryPoint>]
let main argv =
    printf "Imperative Programming Library Management System\n\n"
    initializeFile booksFilePath
    initializeFile membersFilePath
    initializeFile borrowedBooksFilePath

    let mutable running = true
    while running do
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
        
        if choice = "1" then
            printf "Enter Title: "
            let title = Console.ReadLine()
            printf "Enter Author: "
            let author = Console.ReadLine()
            printf "Enter Genre: "
            let genre = Console.ReadLine()
            addBook title author genre
        elif choice = "2" then
            printf "Enter Title: "
            let title = Console.ReadLine()
            removeBook title
        elif choice = "3" then
            printf "Enter Title: "
            let title = Console.ReadLine()
            printf "Enter New Title: "
            let newTitle = Console.ReadLine()
            printf "Enter New Author: "
            let newAuthor = Console.ReadLine()
            printf "Enter New Genre: "
            let newGenre = Console.ReadLine()
            updateBook title newTitle newAuthor newGenre
        elif choice = "4" then
            printf "Enter Search Term (Title, Author, or Genre): "
            let searchTerm = Console.ReadLine()
            searchBooks searchTerm
        elif choice = "5" then
            printf "Enter Name: "
            let name = Console.ReadLine()
            printf "Enter Member ID: "
            let memberId = Console.ReadLine()
            registerMember name memberId
        elif choice = "6" then
            printf "Enter Member ID: "
            let memberId = Console.ReadLine()
            viewMember memberId
        elif choice = "7" then
            printf "Enter Member ID: "
            let memberId = Console.ReadLine()
            printf "Enter New Name: "
            let newName = Console.ReadLine()
            updateMember memberId newName
        elif choice = "8" then
            printf "Enter Title: "
            let title = Console.ReadLine()
            printf "Enter Member ID: "
            let memberId = Console.ReadLine()
            borrowBook title memberId
        elif choice = "9" then
            printf "Enter Title: "
            let title = Console.ReadLine()
            returnBook title
        elif choice = "10" then
            listAvailableBooks()
        elif choice = "11" then
            printf "Enter Member ID: "
            let memberId = Console.ReadLine()
            yourBorrowingBooks memberId
        elif choice = "12" then
            running <- false
            printfn "Exiting Library Management System. Goodbye!"
        else
            printfn "Invalid choice. Please try again."
    0
