mkdir ./test/Documents ./test/Downloads ./test/Music ./test/Pictures ./test/Public ./test/Templates ./test/Videos

echo "
Top Secrect
" > Test.txt

cp -RT Test.txt ./test/Documents/Test.txt
rm Test.txt