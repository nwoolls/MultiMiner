#!/bin/bash

rpc_ip=$1
if [ -z "$rpc_ip" ] ; then
	rpc_ip='127.0.0.1'
fi

rpc_port=$2
if [ -z "$rpc_port" ] ; then
	rpc_port='4028'
fi

rpc_command='devs'
devs_response=`echo -n $rpc_command | netcat $rpc_ip $rpc_port`

function contains() {
    string="$1"
    substring="$2"
    if test "${string#*$substring}" != "$string"
    then
        echo 0
        return 0    # $substring is in $string
    else
    	echo 1
        return 1    # $substring is not in $string
    fi
}

function pct () {
    echo "scale = $3; $1 * 100 / $2" | bc
}

error_status='STATUS=E'
success_status='STATUS=S'

green='\033[0;32m'
red='\033[0;31m'
clear='\033[0m'
lt_gray='\033[0;37m'
dk_gray='\033[0;30m'

if [[ $devs_response = '' ]]; then
	echo -e "${red}FAILURE${clear}: ${rpc_command} RPC call failed (no response from ${rpc_ip}:${rpc_port})"
	exit 1
fi

if [ $(contains "$devs_response" "$error_status") = 0 ] ; then
	echo -e "${red}FAILURE${clear}: ${rpc_command} RPC call failed (${lt_gray}${error_status} returned${clear})"
	exit 1
fi

if [ $(contains "$devs_response" "$success_status") = 0 ] ; then
	echo -e "${dk_gray}[${green}SUCCESS${dk_gray}]${clear} ${rpc_command} RPC call succeeded (${lt_gray}${success_status} returned${clear})"
fi

# get RPC API response into an array split on |
IFS='|'; read -a response_arr <<< "$devs_response"

# start i at 1, skip status entry
for (( i = 1 ; i < ${#response_arr[@]} ; i++ )) do
	rpc_entry=${response_arr[$i]}

	current_hashrate=0
	average_hashrate=0

	IFS=','; read -a entry_arr <<< "$rpc_entry"
	for value_pair in "${entry_arr[@]}"
	do
		IFS='='; read -a pair_entry <<< "$value_pair"

		name=${pair_entry[0]}
		value=${pair_entry[1]}

		if [ $name = 'MHS av' ] ; then
			average_hashrate=$value
		fi

		if [ $name = 'MHS 5s' ] ; then
			current_hashrate=$value
		fi

		if [ $name = 'MHS 20s' ] ; then
			current_hashrate=$value
		fi
	done

	hashrate_ratio=$(pct $current_hashrate $average_hashrate 0)

	if (( 75 < $hashrate_ratio && $hashrate_ratio < 125 )) ; then
		echo -e "${dk_gray}[${green}SUCCESS${dk_gray}]${clear} hashrate ratio is sane (${lt_gray}current=${current_hashrate} average=${average_hashrate} ratio=${hashrate_ratio}${clear})"
	else
		echo -e "${dk_gray}[${red}FAILURE${dk_gray}]${clear} hashrate ratio is NOT sane (${lt_gray}current=${current_hashrate} average=${average_hashrate} ratio=${hashrate_ratio}${clear})"		
	fi

done
