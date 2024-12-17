module Reports

open Utilities
open Book

let listAvailableBooks () =
    let bookList = readJson<Book> booksFilePath
    let availableBooks = filterListByCondition (fun book -> not book.IsBorrowed) bookList
    if availableBooks.IsEmpty then
        printfn "No available books in the library."
    else
        printfn "Available Books:"
        applyToEach (fun (book: Book) ->
            printfn "Title: %s, Author: %s, Genre: %s" book.Title book.Author book.Genre) availableBooks

let yourBorrowingBooks memberId =
    let borrowedBookList = readJson<BorrowedBook> borrowedBooksFilePath
    let memberBorrowings = filterListByCondition (fun borrowedBook -> borrowedBook.MemberId = memberId) borrowedBookList
    if memberBorrowings.IsEmpty then
        printfn "No borrowing history for member ID '%s'." memberId
    else
        printfn "Borrowing History for Member ID '%s':" memberId
        applyToEach (fun borrowedBook ->
            printfn "Title: %s, Borrow Date: %s" borrowedBook.Title (borrowedBook.BorrowDate.ToString("yyyy-MM-dd"))) memberBorrowings
