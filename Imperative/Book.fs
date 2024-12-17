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
    let mutable bookExists = false
    for book in bookList do
        if book.Title = title then
            bookExists <- true
    if bookExists then
        printfn "Book with title '%s' already exists." title
    else
        let newBook = { Title = title; Author = author; Genre = genre; IsBorrowed = false; BorrowDate = None }
        writeJson booksFilePath (newBook :: bookList)
        printfn "Book '%s' added successfully." title



let removeBook title =
    let bookList = readJson<Book> booksFilePath
    let mutable bookToRemove = None
    for book in bookList do
        if book.Title = title then
            bookToRemove <- Some book
    if bookToRemove <> None then
        let book = bookToRemove.Value
        if book.IsBorrowed then
            printfn "Cannot remove book '%s' because it is currently borrowed." title
        else
            let mutable updatedBooks = []
            for b in bookList do
                if b.Title <> title then
                    updatedBooks <- b :: updatedBooks
            writeJson booksFilePath updatedBooks
            printfn "Book '%s' removed successfully." title
    else
        printfn "Book with title '%s' not found." title


let updateBook title newTitle newAuthor newGenre =
    let bookList = readJson<Book> booksFilePath
    let mutable bookToUpdate = None
    for book in bookList do
        if book.Title = title then
            bookToUpdate <- Some book
    if bookToUpdate <> None then
        let book = bookToUpdate.Value
        let updatedBook = { book with Title = newTitle; Author = newAuthor; Genre = newGenre }
        let mutable updatedBooks = []
        for b in bookList do
            if b.Title = title then
                updatedBooks <- updatedBook :: updatedBooks
            else
                updatedBooks <- b :: updatedBooks
        writeJson booksFilePath updatedBooks
        printfn "Book '%s' updated successfully." title
    else
        printfn "Book with title '%s' not found." title

let searchBooks (searchTerm: string) =
    let bookList = readJson<Book> booksFilePath
    let mutable results = []
    for b in bookList do
        if b.Title.Contains(searchTerm) || b.Author.Contains(searchTerm) || b.Genre.Contains(searchTerm) then
            results <- b :: results
    if results.IsEmpty then
        printfn "No books found matching '%s'." searchTerm
    else
        printfn "Search Results:"
        for book in results do
            printfn "Title: %s, Author: %s, Genre: %s, Status: %s"
                    book.Title book.Author book.Genre (if book.IsBorrowed then "Borrowed" else "Available")


let borrowBook title memberId =
    let members = readJson<Member> membersFilePath
    let mutable memberExists = false
    for m in members do
        if m.MemberId = memberId then
            memberExists <- true
    if not memberExists then
        printfn "Member with ID '%s' not found." memberId
    else
        let bookList = readJson<Book> booksFilePath
        let mutable bookToBorrow = None
        for b in bookList do
            if b.Title = title then
                bookToBorrow <- Some b
        if bookToBorrow <> None then
            let book = bookToBorrow.Value
            if book.IsBorrowed then
                printfn "Book '%s' is already borrowed." title
            else
                let updatedBook = { book with IsBorrowed = true; BorrowDate = Some DateTime.Now }
                let mutable updatedBooks = []
                for b in bookList do
                    if b.Title = title then
                        updatedBooks <- updatedBook :: updatedBooks
                    else
                        updatedBooks <- b :: updatedBooks
                writeJson booksFilePath updatedBooks
                let borrowedBooks = readJson<BorrowedBook> borrowedBooksFilePath
                let newBorrowedBook = { Title = title; MemberId = memberId; BorrowDate = DateTime.Now }
                writeJson borrowedBooksFilePath (newBorrowedBook :: borrowedBooks)
                printfn "Book '%s' borrowed successfully on %s." title (updatedBook.BorrowDate.Value.ToString("yyyy-MM-dd"))
        else
            printfn "Book with title '%s' not found." title

let returnBook title =
    let bookList = readJson<Book> booksFilePath
    let mutable bookToReturn = None
    for b in bookList do
        if b.Title = title then
            bookToReturn <- Some b
    if bookToReturn <> None then
        let book = bookToReturn.Value
        if not book.IsBorrowed then
            printfn "Book '%s' is not currently borrowed." title
        else
            let updatedBook = { book with IsBorrowed = false; BorrowDate = None }
            let mutable updatedBooks = []
            for b in bookList do
                if b.Title = title then
                    updatedBooks <- updatedBook :: updatedBooks
                else
                    updatedBooks <- b :: updatedBooks
            writeJson booksFilePath updatedBooks
            let borrowedBooks = readJson<BorrowedBook> borrowedBooksFilePath
            let mutable updatedBorrowedBooks = []
            for bb in borrowedBooks do
                if bb.Title <> title then
                    updatedBorrowedBooks <- bb :: updatedBorrowedBooks
            writeJson borrowedBooksFilePath updatedBorrowedBooks
            printfn "Book '%s' returned successfully." title
    else
        printfn "Book with title '%s' not found." title
