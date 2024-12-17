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
    let mutable memberToView = None
    for m in members do
        if m.MemberId = memberId then
            memberToView <- Some m
    if memberToView <> None then
        let m = memberToView.Value
        printfn "Member Details - Name: %s, Member ID: %s" m.Name m.MemberId
    else
        printfn "Member with ID '%s' not found." memberId


let updateMember memberId newName =
    let members = readJson<Member> membersFilePath
    let mutable memberToUpdate = None
    for m in members do
        if m.MemberId = memberId then
            memberToUpdate <- Some m
    if memberToUpdate <> None then
        let m = memberToUpdate.Value
        let updatedMember = { m with Name = newName }
        let mutable updatedMembers = []
        for m in members do
            if m.MemberId = memberId then
                updatedMembers <- updatedMember :: updatedMembers
            else
                updatedMembers <- m :: updatedMembers
        writeJson membersFilePath updatedMembers
        printfn "Member '%s' updated successfully." memberId
    else
        printfn "Member with ID '%s' not found." memberId
