
interface Message {
    message:string;
}

export const InstructionCommand = ({message}:Message) => {
    return (
        <li className={"font-semibold"}>{message}</li>)
}

