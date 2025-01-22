import { useParams, useNavigate } from "react-router-dom";
import { useEffect } from "react";
export default function ConfirmEmailChange() {

    const navigate = useNavigate()
    const { id, email, code } = useParams()
    async function ChangeEmail() {
        const response = await fetch(`https://localhost:7075/api/Account/ChangeEmail/${id}/${email}/${code}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
        })
        if (response.ok) {
            navigate("/")
        }
        else {
            navigate("/Error")
        }
    }
    useEffect(() => {
        ChangeEmail()
    }, [])
    return(
        <>
        </>
    )
}