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
