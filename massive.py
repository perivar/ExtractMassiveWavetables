import os, sys, struct, shutil, argparse

class Entry(object):
    is_list = False

    def __init__(self, name, data_offset, data_size):
        self.name = name
        self.data_offset = data_offset
        self.data_size = data_size


class EntryList(object):
    is_list = True

    def __init__(self, name, entries):
        self.name = name
        self.entries = entries


def parse_nimd_file(fp):
    magic, empty, total_size = struct.unpack('<4sll', fp.read(12))
    if magic != 'NIMD':
        raise Exception('invalid NIMD header')
    return parse_nimd_entry_list("", fp)


def parse_nimd_entry(fp, indent=''):
    is_list, total_size, name_size = struct.unpack('<bll', fp.read(9))
    name = fp.read(name_size)
    if is_list:
        return parse_nimd_entry_list(name, fp)
    data_offset, data_size = struct.unpack('<ll', fp.read(8))
    return Entry(name, data_offset, data_size)


def parse_nimd_entry_list(name, fp):
    total_size, num_entries = struct.unpack('<ll', fp.read(8))
    entries = []
    for i in range(num_entries):
        entries.append(parse_nimd_entry(fp))
    return EntryList(name, entries)


def extract_nimd_entry(entry, fp, output_dir):
    path = os.path.join(output_dir, entry.name)
    if entry.is_list:
        if not os.path.exists(path):
            os.makedirs(path)
        for sub_entry in entry.entries:
            extract_nimd_entry(sub_entry, fp, path)
    else:
        print "creating file", path
        fp.seek(entry.data_offset)
        with open(path, "wb") as data_fp:
            data_fp.write(fp.read(entry.data_size))


parser = argparse.ArgumentParser()
parser.add_argument('input_file')
parser.add_argument('output_dir')
args = parser.parse_args()

with open(args.input_file, "rb") as fp:
    nimd = parse_nimd_file(fp)
    extract_nimd_entry(nimd, fp, args.output_dir)
	