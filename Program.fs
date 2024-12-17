open System
open System.IO
open System.Text.Json

// Types
type Book = {
    Title: string
    Author: string
    Genre: string
    IsBorrowed: bool
    BorrowDate: DateTime option
}

type Member = {
    Name: string
    MemberId: string
}

type BorrowedBook = {
    Title: string
    MemberId: string
    BorrowDate: DateTime
}

// File paths
let booksFilePath = "books.json"
let membersFilePath = "members.json"
let borrowedBooksFilePath = "borrowedBooks.json"

// Recursive implementations of common list functions
let rec tryFindInList predicate list =
    match list with
    | [] -> None
    | head :: tail -> if predicate head then Some head else tryFindInList predicate tail

let rec mapOverList f list =
    match list with
    | [] -> []
    | head :: tail -> f head :: mapOverList f tail

let rec filterListByCondition predicate list =
    match list with
    | [] -> []
    | head :: tail -> 
        if predicate head then 
            head :: filterListByCondition predicate tail
        else 
            filterListByCondition predicate tail

let rec applyToEach f list =
    match list with
    | [] -> ()
    | head :: tail -> 
        f head
        applyToEach f tail

let rec existsInList predicate list =
    match list with
    | [] -> false
    | head :: tail -> 
        if predicate head then 
            true
        else 
            existsInList predicate tail

// JSON helper functions
let readJson<'T> (filePath: string) : 'T list =
    if File.Exists(filePath) then
        let json = File.ReadAllText(filePath)
        if String.IsNullOrWhiteSpace(json) then
            [] 
        else
            try
                JsonSerializer.Deserialize<'T list>(json)
            with
            | :? System.Text.Json.JsonException ->
                printfn "Warning: Invalid JSON in file '%s'. Returning an empty list." filePath
                []
    else
        [] 

let writeJson<'T> (filePath: string) (data: 'T list) =
    let json = JsonSerializer.Serialize(data)
    File.WriteAllText(filePath, json)

let initializeFile filePath =
    if not (File.Exists(filePath)) then
        File.WriteAllText(filePath, "[]")

// Library modules
module Library =

    module BookManagement =
        let addBook title author genre =
            let bookList = readJson<Book> booksFilePath
            if existsInList (fun (book: Book) -> book.Title = title) bookList then
                printfn "Book with title '%s' already exists." title
            else
                let newBook = { Title = title; Author = author; Genre = genre; IsBorrowed = false; BorrowDate = None }
                writeJson booksFilePath (newBook :: bookList)
                printfn "Book '%s' added successfully." title

        let removeBook title =
            let bookList = readJson<Book> booksFilePath
            match tryFindInList (fun (book: Book) -> book.Title = title) bookList with
            | Some book when book.IsBorrowed ->
                printfn "Book '%s' is currently borrowed and cannot be removed." title
            | Some _ ->
                let updatedBookList = filterListByCondition (fun (book: Book) -> book.Title <> title) bookList
                writeJson booksFilePath updatedBookList
                printfn "Book '%s' removed successfully." title
            | None ->
                printfn "Book with title '%s' not found." title

        let updateBook title newTitle newAuthor newGenre =
            let bookList = readJson<Book> booksFilePath
            match tryFindInList (fun (book: Book) -> book.Title = title) bookList with
            | Some book ->
                let updatedBook = { book with Title = newTitle; Author = newAuthor; Genre = newGenre }
                let updatedBookList = mapOverList (fun (book: Book) -> if book.Title = title then updatedBook else book) bookList
                writeJson booksFilePath updatedBookList
                printfn "Book '%s' updated successfully." title
            | None ->
                printfn "Book with title '%s' not found." title

        let searchBooks searchTerm =
            let bookList = readJson<Book> booksFilePath
            let searchResults = filterListByCondition (fun (book: Book) -> 
                book.Title.Contains(searchTerm: string) || book.Author.Contains(searchTerm: string) || book.Genre.Contains(searchTerm: string)) bookList
            if searchResults.IsEmpty then
                printfn "No books found matching '%s'." searchTerm
            else
                printfn "Search Results:"
                applyToEach (fun (book: Book) ->
                    printfn "Title: %s, Author: %s, Genre: %s, Status: %s" 
                        book.Title book.Author book.Genre (if book.IsBorrowed then "Borrowed" else "Available")) searchResults

    module MemberManagement =
        let registerMember name memberId =
            let memberList = readJson<Member> membersFilePath
            if existsInList (fun (m: Member) -> m.MemberId = memberId) memberList then
                printfn "Member with ID '%s' already exists." memberId
            else
                let newMember = { Name = name; MemberId = memberId }
                writeJson membersFilePath (newMember :: memberList)
                printfn "Member '%s' registered successfully." name

        let viewMember memberId =
            let memberList = readJson<Member> membersFilePath
            match tryFindInList (fun (m: Member) -> m.MemberId = memberId) memberList with
            | Some m ->
                printfn "Member Details - Name: %s, Member ID: %s" m.Name m.MemberId
            | None ->
                printfn "Member with ID '%s' not found." memberId

        let updateMember memberId newName =
            let memberList = readJson<Member> membersFilePath
            match tryFindInList (fun (m: Member) -> m.MemberId = memberId) memberList with
            | Some m ->
                let updatedMember = { m with Name = newName }
                let updatedMemberList = mapOverList (fun (m: Member) -> if m.MemberId = memberId then updatedMember else m) memberList
                writeJson membersFilePath updatedMemberList
                printfn "Member '%s' updated successfully." memberId
            | None ->
                printfn "Member with ID '%s' not found." memberId

    module Borrowing =
        let borrowBook title memberId =
            let memberList = readJson<Member> membersFilePath
            match tryFindInList (fun (m: Member) -> m.MemberId = memberId) memberList with
            | None ->
                printfn "Member with ID '%s' not found." memberId
            | Some _ ->
                let bookList = readJson<Book> booksFilePath
                match tryFindInList (fun (book: Book) -> book.Title = title) bookList with
                | Some book when book.IsBorrowed ->
                    printfn "Book '%s' is already borrowed." title
                | Some book ->
                    let updatedBook = { book with IsBorrowed = true; BorrowDate = Some DateTime.Now }
                    let updatedBookList = mapOverList (fun (book: Book) -> if book.Title = title then updatedBook else book) bookList
                    writeJson booksFilePath updatedBookList
                    let borrowedBookList = readJson<BorrowedBook> borrowedBooksFilePath
                    let newBorrowedBook = { Title = title; MemberId = memberId; BorrowDate = DateTime.Now }
                    writeJson borrowedBooksFilePath (newBorrowedBook :: borrowedBookList)
                    printfn "Book '%s' borrowed successfully." title
                | None ->
                    printfn "Book with title '%s' not found." title

        let returnBook title =
            let bookList = readJson<Book> booksFilePath
            match tryFindInList (fun (book: Book) -> book.Title = title) bookList with
            | Some book when not book.IsBorrowed ->
                printfn "Book '%s' is not currently borrowed." title
            | Some book ->
                let updatedBook = { book with IsBorrowed = false; BorrowDate = None }
                let updatedBookList = mapOverList (fun (book: Book) -> if book.Title = title then updatedBook else book) bookList
                writeJson booksFilePath updatedBookList
                let borrowedBookList = readJson<BorrowedBook> borrowedBooksFilePath
                let updatedBorrowedBookList = filterListByCondition (fun borrowedBook -> borrowedBook.Title <> title) borrowedBookList
                writeJson borrowedBooksFilePath updatedBorrowedBookList
                printfn "Book '%s' returned successfully." title
            | None ->
                printfn "Book with title '%s' not found." title

    module Reports =
        let listAvailableBooks () =
            let bookList = readJson<Book> booksFilePath
            let availableBooks = filterListByCondition (fun book -> not book.IsBorrowed) bookList
            if availableBooks.IsEmpty then
                printfn "No available books in the library."
            else
                printfn "Available Books:"
                applyToEach (fun (book: Book) ->
                    printfn "Title: %s, Author: %s, Genre: %s" book.Title book.Author book.Genre) availableBooks

        let borrowingHistory memberId =
            let borrowedBookList = readJson<BorrowedBook> borrowedBooksFilePath
            let memberBorrowings = filterListByCondition (fun borrowedBook -> borrowedBook.MemberId = memberId) borrowedBookList
            if memberBorrowings.IsEmpty then
                printfn "No borrowing history for member ID '%s'." memberId
            else
                printfn "Borrowing History for Member ID '%s':" memberId
                applyToEach (fun borrowedBook ->
                    printfn "Title: %s, Borrow Date: %s" borrowedBook.Title (borrowedBook.BorrowDate.ToString("yyyy-MM-dd"))) memberBorrowings

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
        Library.BookManagement.addBook title author genre
        mainMenu books members borrowedBooks
    | "2" ->
        printf "Enter Title: "
        let title = Console.ReadLine()
        Library.BookManagement.removeBook title
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
        Library.BookManagement.updateBook title newTitle newAuthor newGenre
        mainMenu books members borrowedBooks
    | "4" ->
        printf "Enter Search Term (Title, Author, or Genre): "
        let searchTerm = Console.ReadLine()
        Library.BookManagement.searchBooks searchTerm
        mainMenu books members borrowedBooks
    | "5" ->
        printf "Enter Name: "
        let name = Console.ReadLine()
        printf "Enter Member ID: "
        let memberId = Console.ReadLine()
        Library.MemberManagement.registerMember name memberId
        mainMenu books members borrowedBooks
    | "6" ->
        printf "Enter Member ID: "
        let memberId = Console.ReadLine()
        Library.MemberManagement.viewMember memberId
        mainMenu books members borrowedBooks
    | "7" ->
        printf "Enter Member ID: "
        let memberId = Console.ReadLine()
        printf "Enter New Name: "
        let newName = Console.ReadLine()
        Library.MemberManagement.updateMember memberId newName
        mainMenu books members borrowedBooks
    | "8" ->
        printf "Enter Title: "
        let title = Console.ReadLine()
        printf "Enter Member ID: "
        let memberId = Console.ReadLine()
        Library.Borrowing.borrowBook title memberId
        mainMenu books members borrowedBooks
    | "9" ->
        printf "Enter Title: "
        let title = Console.ReadLine()
        Library.Borrowing.returnBook title
        mainMenu books members borrowedBooks
    | "10" ->
        Library.Reports.listAvailableBooks()
        mainMenu books members borrowedBooks
    | "11" ->
        printf "Enter Member ID: "
        let memberId = Console.ReadLine()
        Library.Reports.borrowingHistory memberId
        mainMenu books members borrowedBooks
    | "12" ->
        printfn "Exiting Library Management System. Goodbye!"
        ()
    | _ ->
        printfn "Invalid choice. Please try again."
        mainMenu books members borrowedBooks

[<EntryPoint>]

let mainEntry argv =
    initializeFile booksFilePath
    initializeFile membersFilePath
    initializeFile borrowedBooksFilePath

    mainMenu booksFilePath membersFilePath borrowedBooksFilePath
    0