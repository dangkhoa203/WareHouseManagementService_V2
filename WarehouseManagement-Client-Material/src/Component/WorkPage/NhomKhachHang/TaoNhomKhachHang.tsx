import {useEffect, useState} from "react";
import Container from "@mui/material/Container";
import {CircularProgress, Grid2, TextField} from "@mui/material";
import Button from "@mui/material/Button";
import {useNavigate} from "react-router";
interface NhomKhachHangInfo{
    name:string;
    description:string;
}
export default function TaoNhomKhachHang(){
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false)
    const [error, setError] = useState("")
    const [newData, setNewData] = useState<NhomKhachHangInfo>({
        name:"",
        description:"",
    });
    const checkData = () => {
        setError("");
        if (newData.name.length === 0) {
            setError("Bạn chưa nhập đủ thông tin!")
            return false;
        }
        return true;
    }
    const Create = async () => {
        if (checkData()) {
            try {
                setLoading(true)
                const response = await fetch('https://localhost:7075/api/customer-groups', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    credentials: 'include',
                    body: JSON.stringify(newData)
                })
                if (!response.ok) {
                    const content = await response.json();
                    if(content.validateError!==null && !content.validateError.isValid){
                        const list=content.validateError.errors
                        list.forEach((element:any) => {
                            setError(element.errorMessage)
                        })
                    }else {
                        setError(content.errorMessage)
                    }
                }else {
                    navigate("..")
                }
            } catch  {
               console.log("Error")
            }finally {
                setLoading(false)
            }
        }
    }
    const handleNameChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setNewData({...newData,name: e.target.value})
    }
    const handleDescriptionChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setNewData({...newData,description:e.target.value})
    }

    useEffect(()=>{
        document.title = 'Tạo nhóm khách hàng';
    },[])
    return (
        <Container>
            <h2 style={{textAlign:"center"}}>Tạo nhóm nhà sản xuât</h2>
            <Grid2 container spacing={2}>
                <Grid2 size={12}>
                    <Button color="secondary"
                            variant="contained"
                            sx={{width: {xs:"100px",sm:"100px",md:"150px",lg:"250px"}}}
                            onClick={() => navigate(-1)}
                    >Quay về</Button>
                </Grid2>
                <Grid2 size={{xs:12,sm:12,md:4}}>
                    <TextField
                        fullWidth
                        color={"warning"}
                        label="Tên"
                        helperText=""
                        variant="filled"
                        value={newData.name}
                        onChange={handleNameChange}
                    />
                </Grid2>
                <Grid2 size={{xs:12,sm:12,md:8}}>
                    <TextField
                        color="warning"
                        label="Mô tả"
                        fullWidth
                        multiline
                        minRows={4}
                        maxRows={8}
                        defaultValue="Default Value"
                        variant="filled"
                        value={newData.description}
                        onChange={handleDescriptionChange}
                    />
                </Grid2>
                <Grid2 size={12}>
                    <h6>{error}</h6>
                </Grid2>
                <Grid2 size={12} justifyContent="end"  display="flex">
                    <Button disabled={loading} onClick={()=>Create()} variant={loading ? "outlined":"contained"} color="warning" size="large"
                            sx={{width:{xs:"100%",sm:"100%",md:"25%"},fontSize:"1.5em"}}
                    >{loading ? <CircularProgress size={"1.5em"} color="inherit" />:"Tạo"}</Button>
                </Grid2>
            </Grid2>
        </Container>
    )
}