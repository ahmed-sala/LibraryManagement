module Book

open System
open Member
open Utilities

type Book = {
    Title: string
    Author: string
    Genre: string
    IsBorrowed: bool
    BorrowDate: DateTime option
}

type BorrowedBook = {
    Title: string
    MemberId: string
    BorrowDate: DateTime
}

let booksFilePath = "books.json"
let borrowedBooksFilePath = "borrowedBooks.json"

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
