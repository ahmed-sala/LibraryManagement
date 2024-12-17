module Member

open Utilities

type Member = {
    Name: string
    MemberId: string
}

let membersFilePath = "members.json"

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
