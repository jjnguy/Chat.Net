use std::io::prelude::*;
use std::net::TcpStream;
use std::str;

fn main() {
	{
		let mut stream = TcpStream::connect("127.0.0.1:54545").unwrap();

		let mut buf = [0; 1024];
		let mut vec = Vec::new();
		{
		
			let _ = stream.read(&mut buf).unwrap(); // ignore here
			
			for byt in &mut buf[..] {
				if *byt != 0 {
					vec.push(byt);
				}
			}
		}
		
		let mut buf2 = [0; 512];
		let mut i = 0;
		for byt in vec {
			buf2[i] = *byt;
			i = i + 1;
		}
		
		let result = match str::from_utf8(&buf2) {
			Ok(v) => v,
			Err(e) => panic!("Invalid UTF-8 sequence: {}", e),
		};
		
		println!("Data looked like: {}", result.trim_matches('\x00'));
	} // the stream is closed here
}
