====================
PRELUDE:
====================

You can use OpenCL if your video card - from now GPU - supports it.
ATI/AMD, Intel and Nvidia support it through their SDK available at
nvidia, Intel and ATI/AMD website.

Some recent distros have all (proprietry) stuff available as normal
packages.  N.B.  DON'T use X11 opensource drivers provided by your
distribution, only the vendor-supplied drivers support OpenCL.  Either
install fglrx (for old AMD cards) or nvidia dkms package or go directly
with the ones provided by nvidia and ATI.

Notice: Beignet, Mesa, and POCL are not officially supported, but may
be usable in some OpenCL formats.

You can also use OpenCL with CPU, mostly useful if you have several
(or loads of) cores.  This sometimes outperforms the CPU-only formats
due to better scaling than OMP, or due to vectorizing.  See Intel's
and AMD's web sites for drivers.  Note that an Intel driver does
support AMD CPU's and vice versa.

Ensure good cooling; Keep an eye on temperatures.  If the OpenCL runtime
supports it, GPU temperature will be monitored and shown on status lines
and there is a user changeable limit in john.conf that will terminate a
session at 95°C.

This code has been tested on Linux, macOS and Windows, see doc/BUGS for
known issues.

GPU formats won't improve your speed on very short runs due to longer
startup.  It also can't use GPU-side mask generation with "single mode"
so can't be significantly faster than on CPU for the few fastest of
formats in that mode.  For most formats though, "single mode" works fine
nowadays with the only caveats that you might need a whole lot of memory
(you'll get helpful messages if you need to adjust buffer size) and it
might resume pretty poorly (meaning if you stop it and then resume, a
good deal of work will be repeated before actually catching up).


====================
COMPILING:
====================

The new autoconf (./configure) should find your OpenCL installation and
enable it.  If it doesn't, you may need to pass some parameters about where
it's located, e.g.,
    ./configure LDFLAGS=-L/opt/AMDAPP/lib CFLAGS=-I/opt/AMDAPP/include
    make -sj4

To force a build without OpenCL, use:
    ./configure --disable-opencl
    make -sj4


ATI/AMD suggest you to use ATISTREAMSDKROOT env variable to
provide where you have installed their SDK root.
nvidia simply install it in /usr/local/nvidia .

The legacy Makefile assumes you have $ATISTREAMSDKROOT set up to point
to your ATI installation or you have $NVIDIA_CUDA pointing to nvidia
installation.

If in doubt do a

$ updatedb && locate CL/cl.h && locate libOpenCL.so

to locate your path to the includes and libOpenCL.

Adjust NVIDIA_CUDA or ATISTREAMSDKROOT to your needs and
if something is still wrong (but it shouldn't) send
an email to john-users@lists.openwall.com for help.


====================
Supported formats:
====================

See output of "./john --list=formats --format=opencl"


====================
USAGE:
====================

If no --format is given, john will always pick a CPU format.  To use OpenCL
you must explicitly select the format, e.g., --format=wpapsk-opencl


====================
Vectorized formats:
====================

A few formats will ask your device if it runs better vectorized,  and at what
width, and act accordingly.   A vectorized format runs faster on such devices
(notably CPUs,  depending on driver,  and pre-GCN AMD GPUs).  However, a side
effect might be register spilling which will just make it slower.  If a format
defaults  to vectorizing,  --force-scalar  will disable it.  You can also set
ForceScalar = Y  in john.conf to disable it globally.

If/when a format runs vectorized, it will show algorithm name as e.g.
[OpenCL 4x] as opposed to just [OpenCL].


====================
Work size tuning:
====================

All OpenCL formats will auto-tune to best speed, limited by things like
device memory or total duration for a batch of passwords.  You can override
the auto-tune using the command-line options -lws=N and/or -gws=N for local
and global work sizes respectively.  If one is given, the other will be auto-
tuned.  As an alternative, environment variables LWS and GWS can be used
instead, with the difference that the latter won't be stored to a session
file.  If both are used, the command-line options silently take precedence
over the environment variables.


====================
Watchdog Timer:
====================


If your GPU is also your active display device, a watchdog timer is enabled
by default - killing any kernel that runs for more than about five seconds
(nvidia) or two seconds (AMD).  You will normally not get a proper error
message, just some kind of failure after five seconds or more, like:

  OpenCL error (CL_INVALID_COMMAND_QUEUE) in file (OpenCL_encfs_fmt.c) (...)

Our goal is to split such kernels into subkernels with shorter durations but
in the meantime (and especially if running slow kernels on weak devices) you
might need to disable this watchdog.  For nvidia cards, you can check this
setting using "--list=OpenCL-devices".  Example output:

    Platform #0 name: NVIDIA CUDA, version: OpenCL 1.1 CUDA 4.2.1
        Device #0 (1) name:     GeForce GT 650M
        Device vendor:          NVIDIA Corporation
        Device type:            GPU (LE)
        Device version:         OpenCL 1.1 CUDA
        Driver version:         304.51
        Global Memory:          1023.10 MB
        Global Memory Cache:    32.0 KB
        Local Memory:           48.0 KB (Local)
        Max clock (MHz) :       900
        Max Work Group Size:    1024
        Parallel compute cores: 2
        Stream processors:      384  (2 x 192)
        Warp size:              32
        Max. GPRs/work-group:   65536
        Compute capability:     3.0 (sm_30)
        Kernel exec. timeout:   yes            <-- enabled watchdog

This particular output is not always available under macOS.  We are
currently not aware of any way to disable this watchdog under macOS.  Under
Linux (and possibly other systems using X), you can disable it for nvidia
cards by adding the 'Option "Interactive"' line to /etc/X11/xorg.conf:

    Section "Device"
        Identifier     "Device0"
        Driver         "nvidia"
        VendorName     "NVIDIA Corporation"
        Option         "Interactive"        "False"
    EndSection

At this time we are not aware of any way to check or change this for AMD cards.
What we do know is that some old AMD drivers will crash after repeated runs of
as short durations as 200 ms, necessating a reboot.  If this happens, just
upgrade your driver.


=====================
Multi-device support:
=====================

Currently only mscash2-OpenCL support multiple devices by itself.  However,
all other formats can use it together with MPI or the --fork option.  For
example, let's say you have four GPU or accelerator cards in your local host:

$ ./john -fork=4 -dev=gpu,acc -format=(...)

The above will fork to four processes and each process will use a different
GPU or Accelerator device.  The "-dev" option (--device) is likely needed
because it defaults to 'all' which may include unwanted devices.  Instead
of -dev=gpu,acc (use all/any GPUs and accelerators) you could specify them
explicitly if needed, e.g. -dev=1,2,6,7.

Or maybe you have two cards in a remote host called alpha and one card in
a host called bravo.  Build with MPI support and use this variant of the above:

$ mpirun -host alpha,alpha,bravo ./john -dev=gpu,acc -format=(...)

The above will start two processes on alpha, using different GPUs, as well
as one process on bravo.  In this case, the "-dev=gpu" option will be
enumerated on each host so if GPUs are devices 2 & 4 on alpha but device 1 on
bravo, that is not a problem.

If for some reason you want to run e.g.  two processes on each GPU, just double
the -fork argument or the MPI number of hosts (using -np option to mpirun).
The device list will round-robin.
