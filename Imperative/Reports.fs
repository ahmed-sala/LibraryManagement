module Reports

open Utilities
open Book
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

let yourBorrowingBooks memberId =
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
