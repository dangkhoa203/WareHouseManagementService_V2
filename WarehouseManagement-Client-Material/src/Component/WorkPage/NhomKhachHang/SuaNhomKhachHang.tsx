import {useEffect, useState} from "react";
import Container from "@mui/material/Container";
import {Grid2, TextField} from "@mui/material";
import {useParams} from "react-router";
import Button from "@mui/material/Button";
interface NhomKhachHangInfo{
    id:string;
    name:string;
    description:string;
}

export default function SuaNhomKhachHang(){
    const {id} = useParams() as { id: string };
    const [loading, setLoading] = useState(false)
    const [newData, setNewData] = useState<NhomKhachHangInfo>({
        id:id,
        name:"",
        description:"",
    });
    const Update = async () => {
        // if (checkData(data)) {
        //     try {
        //         setLoading(true)
        //         const response = await fetch('https://warehouseservice.azurewebsites.net/api/customer-groups', {
        //             method: 'POST',
        //             headers: {
        //                 'Content-Type': 'application/json',
        //             },
        //             credentials: 'include',
        //             body: JSON.stringify(data)
        //         })
        //         if (!response.ok) {
        //             const content = await response.json();
        //             if(content.validateError!==null && !content.validateError.isValid){
        //                 let list=content.validateError.errors
        //                 list.forEach((element) => {
        //                     setError(element.errorMessage)
        //                 })
        //             }else {
        //                 setError(content.errorMessage)
        //             }
        //             setLoading(false)
        //             throw Error(content.errorMessage);
        //         }
        //         setLoading(false)
        //         navigate("/NhomKhachHang")
        //     } catch (e) {
        //         setLoading(false)
        //     }
        // }
    }
    useEffect(()=>{
        document.title = `Sửa ${id}`;
    },[])
    return (
        <Container>
            <h2 style={{textAlign:"center"}}>Sửa nhóm khách hàng {id}</h2>
            <Grid2 container spacing={2}>
                <Grid2 size={{xs:12,sm:12,md:4}}>
                    <TextField
                        fullWidth
                        color={"warning"}
                        label="Tên"
                        helperText=""
                        variant="filled"
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
                    />
                </Grid2>
                <Grid2 size={12} justifyContent="end"  display="flex">
                    <Button variant="outlined" color="warning" size="large"
                            sx={{width:{xs:"100%",sm:"100%",md:"25%"}}}
                    >Tạo</Button>
                </Grid2>
            </Grid2>
        </Container>
    )
}