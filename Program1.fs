open System
open System.IO
open System.Text.Json

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

let booksFilePath = "books.json"
let membersFilePath = "members.json"
let borrowedBooksFilePath = "borrowedBooks.json"

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

let addBook title author genre =
    let books = readJson<Book> booksFilePath
    let bookExists = ref false
    for book in books do
        if book.Title = title then
            bookExists.Value <- true
    if bookExists.Value then
        printfn "Book with title '%s' already exists." title
    else
        let newBook = { Title = title; Author = author; Genre = genre; IsBorrowed = false; BorrowDate = None }
        writeJson booksFilePath (newBook :: books)
        printfn "Book '%s' added successfully." title

let removeBook title =
    let books = readJson<Book> booksFilePath
    let bookToRemove = ref None
    for book in books do
        if book.Title = title then
                    bookToRemove.Value <- Some book
    match bookToRemove.Value with
    | Some book when book.IsBorrowed ->
        printfn "Cannot remove book '%s' because it is currently borrowed." title
    | Some _ ->
        let updatedBooks = 
            let mutable result = []
            for b in books do
                if b.Title <> title then
                    result <- b :: result
            result
        writeJson booksFilePath updatedBooks
        printfn "Book '%s' removed successfully." title
    | None ->
        printfn "Book with title '%s' not found." title

let updateBook title newTitle newAuthor newGenre =
    let books = readJson<Book> booksFilePath
    let bookToUpdate = ref None
    for book in books do
        if book.Title = title then
            bookToUpdate.Value <- Some book
    match bookToUpdate.Value with
    | Some book ->
        let updatedBook = { book with Title = newTitle; Author = newAuthor; Genre = newGenre }
        let updatedBooks = 
            let mutable result = []
            for b in books do
                if b.Title = title then
                    result <- updatedBook :: result
                else
                    result <- b :: result
            result
        writeJson booksFilePath updatedBooks
        printfn "Book '%s' updated successfully." title
    | None ->
        printfn "Book with title '%s' not found." title

let searchBooks (searchTerm: string) =
    let books = readJson<Book> booksFilePath
    let results = 
        let mutable result = []
        for b in books do
            if b.Title.Contains(searchTerm) || b.Author.Contains(searchTerm) || b.Genre.Contains(searchTerm) then
                result <- b :: result
        result
    if results.IsEmpty then
        printfn "No books found matching '%s'." searchTerm
    else
        printfn "Search Results:"
        for book in results do
            printfn "Title: %s, Author: %s, Genre: %s, Status: %s"
                    book.Title book.Author book.Genre (if book.IsBorrowed then "Borrowed" else "Available")

let registerMember name memberId =
    let members = readJson<Member> membersFilePath
    let memberExists = ref false
    for m in members do
        if m.MemberId = memberId then
            memberExists.Value <- true
    if memberExists.Value then
        printfn "Member with ID '%s' already exists." memberId
    else
        let newMember = { Name = name; MemberId = memberId }
        writeJson membersFilePath (newMember :: members)
        printfn "Member '%s' registered successfully." name

let viewMember memberId =
    let members = readJson<Member> membersFilePath
    let memberToView = ref None
    for m in members do
        if m.MemberId = memberId then
            memberToView.Value <- Some m
    match memberToView.Value with
    | Some m ->
        printfn "Member Details - Name: %s, Member ID: %s" m.Name m.MemberId
    | None ->
        printfn "Member with ID '%s' not found." memberId

let updateMember memberId newName =
    let members = readJson<Member> membersFilePath
    let memberToUpdate = ref None
    for m in members do
        if m.MemberId = memberId then
            memberToUpdate.Value <- Some m
    match memberToUpdate.Value with
    | Some m ->
        let updatedMember = { m with Name = newName }
        let updatedMembers = 
            let mutable result = []
            for m in members do
                if m.MemberId = memberId then
                    result <- updatedMember :: result
                else
                    result <- m :: result
            result
        writeJson membersFilePath updatedMembers
        printfn "Member '%s' updated successfully." memberId
    | None ->
        printfn "Member with ID '%s' not found." memberId

let borrowBook title memberId =
    let members = readJson<Member> membersFilePath
    let memberExists = ref false
    for m in members do
        if m.MemberId = memberId then
            memberExists.Value <- true
    if not memberExists.Value then
        printfn "Member with ID '%s' not found." memberId
    else
        let books = readJson<Book> booksFilePath
        let bookToBorrow = ref None
        for b in books do
            if b.Title = title then
                bookToBorrow.Value <- Some b       
        match bookToBorrow.Value with
        | Some b when b.IsBorrowed ->
            printfn "Book '%s' is already borrowed." title
        | Some b ->
            let updatedBook = { b with IsBorrowed = true; BorrowDate = Some DateTime.Now }
            let updatedBooks = 
                let mutable result = []
                for b in books do
                    if b.Title = title then
                        result <- updatedBook :: result
                    else
                        result <- b :: result
                result
            writeJson booksFilePath updatedBooks
            let borrowedBooks = readJson<BorrowedBook> borrowedBooksFilePath
            let newBorrowedBook = { Title = title; MemberId = memberId; BorrowDate = DateTime.Now }
            writeJson borrowedBooksFilePath (newBorrowedBook :: borrowedBooks)
            printfn "Book '%s' borrowed successfully on %s." title (updatedBook.BorrowDate.Value.ToString("yyyy-MM-dd"))
        | None ->
            printfn "Book with title '%s' not found." title

let returnBook title =
    let books = readJson<Book> booksFilePath
    let bookToReturn = ref None
    for b in books do
        if b.Title = title then
            bookToReturn.Value <- Some b
    match bookToReturn.Value with
    | Some b when not b.IsBorrowed ->
        printfn "Book '%s' is not currently borrowed." title
    | Some b ->
        let updatedBook = { b with IsBorrowed = false; BorrowDate = None }
        let updatedBooks = 
            let mutable result = []
            for b in books do
                if b.Title = title then
                    result <- updatedBook :: result
                else
                    result <- b :: result
            result
        writeJson booksFilePath updatedBooks
        let borrowedBooks = readJson<BorrowedBook> borrowedBooksFilePath
        let updatedBorrowedBooks = 
            let mutable result = []
            for bb in borrowedBooks do
                if bb.Title <> title then
                    result <- bb :: result
            result
        writeJson borrowedBooksFilePath updatedBorrowedBooks
        printfn "Book '%s' returned successfully." title
    | None ->
        printfn "Book with title '%s' not found." title

let listAvailableBooks () =
    let books = readJson<Book> booksFilePath
    let availableBooks = 
        let mutable result = []
        for b in books do
            if not b.IsBorrowed then
                result <- b :: result
        result
    if availableBooks.IsEmpty then
        printfn "No available books in the library."
    else
        printfn "Available Books:"
        for b in availableBooks do
            printfn "Title: %s, Author: %s, Genre: %s" b.Title b.Author b.Genre

let borrowingHistory memberId =
    let borrowedBooks = readJson<BorrowedBook> borrowedBooksFilePath
    let memberBorrowings = 
        let mutable result = []
        for bb in borrowedBooks do
            if bb.MemberId = memberId then
                result <- bb :: result
        result
    if memberBorrowings.IsEmpty then
        printfn "No borrowing history for member ID '%s'." memberId
    else
        printfn "Borrowing History for Member ID '%s':" memberId
        for bb in memberBorrowings do
            printfn "Title: %s, Borrow Date: %s" bb.Title (bb.BorrowDate.ToString("yyyy-MM-dd"))

[<EntryPoint>]
let main argv =
printf "Imparative Programming Library Management System\n\n"
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
