module Member

open Utilities

type Member = {
    Name: string
    MemberId: string
}

let membersFilePath = "members.json"

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

