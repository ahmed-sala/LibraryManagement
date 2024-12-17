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

    let running = ref true
    while running.Value do
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
        printfn "11. Borrowing History"
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
            addBook title author genre
        | "2" ->
            printf "Enter Title: "
            let title = Console.ReadLine()
            removeBook title
        | "3" ->
            printf "Enter Title: "
            let title = Console.ReadLine()
            printf "Enter New Title: "
            let newTitle = Console.ReadLine()
            printf "Enter New Author: "
            let newAuthor = Console.ReadLine()
            printf "Enter New Genre: "
            let newGenre = Console.ReadLine()
            updateBook title newTitle newAuthor newGenre
        | "4" ->
            printf "Enter Search Term (Title, Author, or Genre): "
            let searchTerm = Console.ReadLine()
            searchBooks searchTerm
        | "5" ->
            printf "Enter Name: "
            let name = Console.ReadLine()
            printf "Enter Member ID: "
            let memberId = Console.ReadLine()
            registerMember name memberId
        | "6" ->
            printf "Enter Member ID: "
            let memberId = Console.ReadLine()
            viewMember memberId
        | "7" ->
            printf "Enter Member ID: "
            let memberId = Console.ReadLine()
            printf "Enter New Name: "
            let newName = Console.ReadLine()
            updateMember memberId newName
        | "8" ->
            printf "Enter Title: "
            let title = Console.ReadLine()
            printf "Enter Member ID: "
            let memberId = Console.ReadLine()
            borrowBook title memberId
        | "9" ->
            printf "Enter Title: "
            let title = Console.ReadLine()
            returnBook title
        | "10" ->
            listAvailableBooks()
        | "11" ->
            printf "Enter Member ID: "
            let memberId = Console.ReadLine()
            borrowingHistory memberId
        | "12" ->
            running.Value <- false
            printfn "Exiting Library Management System. Goodbye!"
        | _ ->
            printfn "Invalid choice. Please try again."
    0
