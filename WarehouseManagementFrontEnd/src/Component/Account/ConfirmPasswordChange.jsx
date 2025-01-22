import { useParams, useNavigate } from "react-router-dom";
import { useEffect } from "react";
export default function ConfirmPasswordChange() {

    const navigate = useNavigate()
    const { id, password, code } = useParams()
    async function ChangePassword() {
        const response = await fetch(`https://localhost:7075/api/Account/ChangePassword/${id}/${password}/${code}`, {
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
        ChangePassword()
    }, [])
    return(
        <>
        </>
    )
}